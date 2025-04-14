# CapstoneGroup3

A ticketing system for users to manage projects and groups. Projects have boards with customizable stages to progress tickets through.

---

## 🚀 Startup Instructions

1. **Reset Local Database (if it exists)**  
   Open **SQL Server Object Explorer** and check if `TicketAppDB` exists. If it does, delete it.

2. **Update the Database**  
   Open the **NuGet Package Manager Console** and run: update-database
   
3. **Verify Database Creation**  
Confirm that `TicketAppDB` was created in SQL Server Object Explorer.

4. **Seed the Database**  
Copy the contents of: CapstoneGroup3/code/SQL Scripts/SeedScript.txt

5. **Execute the Seed Script**  
In SQL Server Object Explorer, right-click `TicketAppDB` → **New Query**, paste the script, and run it.  
If successful, the output should show that many rows were modified.

6. **Login as a Seeded User**

---

## 👤 Default Credentials

- **Admin**
- Username: `Admin`
- Password: `Admin123!`

- **Development Users**  
All follow this pattern:
- Username: `FirstnameLastname` (e.g., `JohnSmith`)
- Password: `FirstnameLastname123!` (e.g., `JohnSmith123!`)