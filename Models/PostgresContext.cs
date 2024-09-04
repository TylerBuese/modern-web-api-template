using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace template.Models;

public partial class PostgresContext : DbContext
{	
	public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }
	
}
