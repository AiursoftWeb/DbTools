using Demo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.PostgreSql;

public class PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : DemoDbContext(options);
