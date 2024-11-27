#### Instructions for Running the Code
1. Clone the repository from GitHub.
2. Install dependencies using dotnet restore.
3. Run the application using dotnet run.
4. Use tools like Postman to test the endpoints:
      a. POST /api/shorten with { "OriginalUrl": "http://example.com" }.
      b. GET /api/{short_id} to resolve the shortened URL.


#### Here’s a breakdown of how the solution can be scaled using databases, caching, and distributed storage:

---

### **1. Database Integration**
#### **Choose the Right Database**
- **Relational Databases (e.g., PostgreSQL, MySQL)**:
  - Store mappings between short IDs and original URLs in a table with indexed columns.
  - Suitable for transactional consistency and smaller-scale systems.
  - Schema example:
    ```sql
    CREATE TABLE UrlMappings (
        ShortId VARCHAR(10) PRIMARY KEY,
        OriginalUrl TEXT NOT NULL,
        CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
    ```
- **NoSQL Databases (e.g., DynamoDB, MongoDB, Cassandra)**:
  - Designed for horizontal scaling and high-speed lookups.
  - Store key-value pairs (`ShortId -> OriginalUrl`) for faster reads.
  - Example:
    ```json
    {
      "ShortId": "abc123",
      "OriginalUrl": "http://example.com",
      "CreatedAt": "2024-11-26T12:00:00Z"
    }
    ```

#### **Sharding and Partitioning**
- Use sharding to distribute data across multiple database nodes.
- Partition data by hash of the short ID or the domain to balance the load.

---

### **2. Caching**
#### **Purpose of Caching**
- Reduce database read load by caching frequently accessed data.
- Improve the latency of URL resolution.

#### **Tools**
- **In-Memory Caches**: Use tools like **Redis** or **Memcached** for storing short ID-to-URL mappings temporarily.
- **Integration**:
  - Query the cache first for a short ID.
  - If not found, fetch from the database and populate the cache.
  - Use TTL (time-to-live) to periodically refresh cache entries.

#### **Code Example**
```csharp
public class CachedUrlShortenerService : IUrlShortenerService
{
    private readonly IUrlMappingRepository _repository;
    private readonly ICacheService _cache;

    public CachedUrlShortenerService(IUrlMappingRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public string ResolveUrl(string shortId)
    {
        // Check cache first
        if (_cache.TryGetValue(shortId, out var originalUrl))
        {
            return originalUrl;
        }

        // Fallback to database
        var url = _repository.GetOriginalUrl(shortId);

        // Add to cache
        _cache.Set(shortId, url, TimeSpan.FromMinutes(10));

        return url;
    }
}
```

---

### **3. Distributed Storage**
#### **When to Use Distributed Storage**
- Required for massive-scale systems where millions of mappings need to be stored.
- Examples:
  - **Amazon DynamoDB**: Scalable NoSQL solution.
  - **Google Bigtable**: Optimized for large datasets.
  - **Cassandra**: Open-source distributed database.

#### **Design**
- Use a consistent hashing algorithm to distribute short ID mappings across nodes.
- Store the mappings redundantly across multiple nodes for fault tolerance.

---

### **4. URL Generation and Collision Handling**
#### **Unique ID Generation**
- Use a **hashing function** (e.g., SHA256, MurmurHash) to generate short IDs based on the original URL.
  - Reduces the chance of collisions for the same URL.
- Use **distributed ID generators** (e.g., Twitter Snowflake or UUIDs) for unique short ID generation in distributed environments.

#### **Collision Handling**
- Maintain a database constraint on the `ShortId` column to avoid duplicates.
- In case of a collision, retry ID generation.

---

### **5. Handling High Traffic**
#### **Horizontal Scaling**
- Deploy multiple instances of the service behind a load balancer (e.g., AWS ELB, Nginx).
- Use container orchestration tools like **Kubernetes** or **Docker Swarm** to manage instances.

#### **Rate Limiting and Throttling**
- Prevent abuse by implementing rate limiting for API endpoints.
- Use tools like **Azure API Management**, **AWS API Gateway**, or custom middleware.

---

### **6. Monitoring and Logging**
#### **Metrics**
- Monitor API usage, latency, error rates, and cache hit/miss ratios using tools like **Prometheus** or **Datadog**.

#### **Logging**
- Use centralized logging systems like **ELK Stack (Elasticsearch, Logstash, Kibana)** or **Azure Application Insights** for debugging and analytics.

---

### **7. Security Considerations**
#### **Prevent Malicious URLs**
- Validate URLs to avoid hosting phishing or malicious content.
- Implement domain allow/block lists.

#### **SSL Encryption**
- Serve all endpoints over HTTPS to ensure secure communication.

---

### **8. Scalability Example Architecture**
![example](https://github.com/user-attachments/assets/d5177c77-5fea-4213-aff3-e293987d6625)


#### **Components**
1. **API Gateway**: Handles client requests and enforces rate limits.
2. **Load Balancer**: Distributes traffic among application servers.
3. **App Servers**: Hosts the URL shortening logic and service.
4. **Cache Layer**: Reduces database load for frequently accessed mappings.
5. **Database**: Stores the mappings persistently.
6. **Monitoring & Logging**: Tracks performance and errors.

---

### **Conclusion**
This scalable solution leverages databases, caching, and distributed systems to handle high traffic efficiently. It also ensures reliability and performance through proper architecture, monitoring, and security measures.

