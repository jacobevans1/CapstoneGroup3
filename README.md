# CapstoneGroup3
A ticketing system for users to manage projects and groups.
Projects have boards with customizable stages to progress tickets thorugh.

--- Startup ---

Step 1: Check to see if the local DB, TicketAppDB exists in the SQL Server Object Explorer. If it does exist, delete the local DB.
Step 2: Once the local DB has been deleted or if it wasn't found originally, run the update-database command in the Nuget Package Manager console.
Step 3: Check to see if the TicketAppDB was created in the SQL Server Object Explorer.
Step 4: If TicketAppDB was created sucessfully, copy the script from ...\CapstoneGroup3\code\SQL Scripts\SeedScript.txt
Step 5: Run the copied script from "SeedScript.txt" on the local database created in step 2 by right clicking the 
		TicketAppDB found in the SQL Server Object Explorer and selecting "New Query" option. 
		Press the run query button at the top left of the query window. If sucessfull, the output should show that many rows were modified.
Step 6: Login as your desired user.

There will be a seeded admin user with the credentials:
Username: Admin
Password: Admin123!

All user credentials in the application will follow this pattern during development:
Username: FirstnameLastname (EX: JohnSmith, or JaneDoe)
Password: FirstnameLastname123!

EX:
Username: JohnSmith
Password: JohnSmith123!
