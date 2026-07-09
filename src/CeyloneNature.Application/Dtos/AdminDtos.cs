namespace CeyloneNature.Application.Dtos;

public class AdminStatsDto
{
    public decimal TotalRevenue { get; set; }
    public double RevenueChange { get; set; }
    public int TotalOrders { get; set; }
    public double OrdersChange { get; set; }
    public int ActiveCustomers { get; set; }
    public double CustomersChange { get; set; }
}

public class RevenueDataPointDto
{
    public string Week { get; set; } = "";
    public decimal Revenue { get; set; }
}

public class InventoryAlertDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = "";
    public string Image { get; set; } = "";
    public int StockCount { get; set; }
    public string Status { get; set; } = "";
}

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Initials { get; set; } = "";
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime JoinedDate { get; set; }
    public string Status { get; set; } = "active";
}
