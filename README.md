# ![DuckSQL Logo](DuckSQL/Resources/DuckSQL-Logo-Transparent.ico) DuckSQL 

A lightweight Windows desktop SQL query tool powered by [DuckDB](https://duckdb.org/). DuckSQL makes it easy to query data files (Parquet, CSV, JSON, XLSX, and more) using SQL without the need for a traditional database server.

<img width="1070" height="628" alt="image" src="https://github.com/user-attachments/assets/79d5cf4a-f71d-48b6-bd67-8eaa4619fdb0" />

## Features

- **Query Multiple File Formats**: Execute SQL queries directly against Parquet, CSV, JSON, Excel, and other data files
- **In-Memory Processing**: Leverages DuckDB's high-performance in-memory analytics engine
- **Multi-Tab Interface**: Work with multiple queries simultaneously in separate tabs
- **Syntax Highlighting**: SQL syntax highlighting for better code readability
- **Quick Results**: View query results in a customizable data grid
- **Export Results**: Save query results to CSV, Excel, or JSON format
- **Drag & Drop Support**: Simply drag files or folders into the application to start querying

## Download

Download the latest release from the [Releases](https://github.com/mukunku/DuckSQL/releases) page

Also check out our [User Guide](https://github.com/mukunku/DuckSQL/wiki)

### Quick Start

1. **Open a data file**: Go to `File > Open Data File` or drag & drop a file into the window
  * Supported data file types: csv, parquet, xlsx, json
2. **Write your query**: Enter SQL in the query editor
   ```sql
   SELECT *
   FROM 'your-file.parquet'
   LIMIT 100
   ```
3. **Execute**: Press F5 or click the Execute button
4. **View results**: Results appear in the grid below the query editor

### Example Queries

Query a Parquet file:
```sql
SELECT column1, column2, COUNT(*)
FROM 'data.parquet'
GROUP BY column1, column2
ORDER BY COUNT(*) DESC
```

Query a CSV file:
```sql
SELECT *
FROM 'data.csv'
WHERE column1 > 100
```

Query multiple files:
```sql
SELECT *
FROM 'folder/*.parquet'
WHERE date >= '2024-01-01'
```

# Analytics
Users can opt-in to share anonymous usage data to help make the app better. [^1]

Checkout the [DuckSQL Analytics Dashboard](https://app.amplitude.com/analytics/share/7207c0b64c154e979afd7082980d6dd6) if you're interested!

[^1]: Full privacy policy here: https://github.com/mukunku/DuckSQL/wiki/Privacy-Policy

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.
