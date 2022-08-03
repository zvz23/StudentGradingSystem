public class StudentAccount {
    public int StudentAccountId { get; set; }
    public string SchoolId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; } = false;
    
}