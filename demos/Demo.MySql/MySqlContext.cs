using Demo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.MySql;

public class MySqlContext(DbContextOptions<MySqlContext> options) : DemoDbContext(options);
