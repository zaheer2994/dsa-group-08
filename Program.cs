using System;

namespace HotelManagementSystem
{
    // Custom LinkedList Node
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    // Custom Linked Listt
    public class CustomLinkedList<T>
    {
        public Node<T> Head { get; set; }
        public Node<T> Tail { get; set; }
        public int Count { get; set; }

        public CustomLinkedList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public void AddLast(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                Tail = newNode;
            }
            Count++;
        }

        public void Clear()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public void Remove(Node<T> node)
        {
            if (node == null || Head == null)
                return;

            if (node == Head)
            {
                Head = Head.Next;
                if (Head == null)
                    Tail = null;
                Count--;
                return;
            }

            Node<T> current = Head;
            while (current.Next != null)
            {
                if (current.Next == node)
                {
                    current.Next = node.Next;
                    if (node == Tail)
                        Tail = current;
                    Count--;
                    return;
                }
                current = current.Next;
            }
        }

        public Node<T> First => Head;
    }

    static class Utility
    {
        public static int GetIntInput(string message)
        {
            int input;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.Write("Invalid input. Please enter a valid number: ");
            }
            return input;
        }

        public static decimal GetDecimalInput(string message)
        {
            decimal input;
            Console.Write(message);
            while (!decimal.TryParse(Console.ReadLine(), out input))
            {
                Console.Write("Invalid input. Please enter a valid decimal: ");
            }
            return input;
        }

        public static string GetNonEmptyString(string message)
        {
            string? input;
            do
            {
                Console.Write(message);
                input = Console.ReadLine()?.Trim();
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static bool GetBoolInput(string message)
        {
            bool input;
            Console.Write(message);
            while (!bool.TryParse(Console.ReadLine(), out input))
            {
                Console.Write("Invalid input. Please enter 'true' or 'false': ");
            }
            return input;
        }
    }
    // Customer Class
    public class Customer
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string RoomNumber { get; set; }

        public Customer(string name, string contactNumber, string roomNumber)
        {
            Name = name;
            ContactNumber = contactNumber;
            RoomNumber = roomNumber;
        }

        public override string ToString()
        {
            return $"Customer: {Name}, Contact: {ContactNumber}, Room: {RoomNumber}";
        }
    }

    // CustomerManagement Class
    public class CustomerManagement
    {
        private CustomLinkedList<Customer> customers = new CustomLinkedList<Customer>();

        // Public property to expose the customers list
        public CustomLinkedList<Customer> Customers => customers;

        // Add a customer and sort the list
        public void AddCustomer(string name, string contactNumber, string roomNumber)
        {
            customers.AddLast(new Customer(name, contactNumber, roomNumber));
            InsertionSortCustomers(); // Sort the entire list after adding a new customer
        }

        // Display all customers
        public void DisplayCustomers()
        {
            InsertionSortCustomers(); // Ensure the list is sorted before displaying
            Node<Customer> current = customers.First;
            while (current != null)
            {
                Console.WriteLine(current.Data);
                current = current.Next;
            }
        }

        // Insertion Sort for customers by name
        public void InsertionSortCustomers()
        {
            if (customers.First == null || customers.First.Next == null)
                return;

            Node<Customer> sorted = null; // Initialize sorted list
            Node<Customer> current = customers.First;

            while (current != null)
            {
                Node<Customer> next = current.Next; // Store the next node

                // Insert the current node into the sorted list
                if (sorted == null || CompareStringsLexicographically(sorted.Data.Name, current.Data.Name) >= 0)
                {
                    // Insert at the beginning
                    current.Next = sorted;
                    sorted = current;
                }
                else
                {
                    // Traverse the sorted list to find the correct position
                    Node<Customer> temp = sorted;
                    while (temp.Next != null && CompareStringsLexicographically(temp.Next.Data.Name, current.Data.Name) < 0)
                    {
                        temp = temp.Next;
                    }
                    current.Next = temp.Next;
                    temp.Next = current;
                }

                current = next; // Move to the next node
            }

            customers.Head = sorted; // Update the head of the list
        }

        private int CompareStringsLexicographically(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            int minLength = Math.Min(len1, len2);

            // Convert strings to lowercase for case-insensitive comparison
            str1 = str1.ToLower();
            str2 = str2.ToLower();

            for (int i = 0; i < minLength; i++)
            {
                if (str1[i] < str2[i])
                    return -1;
                if (str1[i] > str2[i])
                    return 1;
            }

            if (len1 < len2)
                return -1;
            if (len1 > len2)
                return 1;

            return 0;
        }
    }

    // Room Class
    public class Room
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; } // Luxury or Normal
        public bool IsAvailable { get; set; }
        public int PricePerNight { get; set; }

        public Room(string roomNumber, string type, bool isAvailable, int price)
        {
            RoomNumber = roomNumber;
            Type = type;
            IsAvailable = isAvailable;
            PricePerNight = price;
        }

        public override string ToString()
        {
            return $"Room {RoomNumber}: {Type} - {(IsAvailable ? "Available" : "Booked")} - ${PricePerNight}/night";
        }
    }

    // HotelManagement Class
    public class HotelManagement
    {
        private CustomLinkedList<Room> rooms = new CustomLinkedList<Room>();
        private CustomLinkedList<string> customerBookings = new CustomLinkedList<string>(); // RoomNumber → CustomerName mapping
        private CustomerManagement customerManagement;

        public HotelManagement(CustomerManagement customerManagement)
        {
            this.customerManagement = customerManagement;

            // Add rooms only once during initialization
            AddRoom("001", "Luxury Suite", false, 300); // Already booked
            AddRoom("002", "Luxury Suite", false, 300); // Already booked
            AddRoom("003", "Normal Room", true, 100);
            AddRoom("004", "Normal Room", true, 100);
            AddRoom("005", "Normal Room", true, 100);
            AddRoom("101", "Luxury Suite", true, 300);
            AddRoom("102", "Luxury Suite", true, 300);
            AddRoom("103", "Normal Room", true, 100);
            AddRoom("104", "Normal Room", false, 100); // Already booked
            AddRoom("105", "Normal Room", true, 100);
        }

        // Add a room dynamically
        public void AddRoom(string roomNumber, string type, bool isAvailable, int price)
        {
            rooms.AddLast(new Room(roomNumber, type, isAvailable, price));
        }

        // Display available rooms
        public void DisplayRooms()
        {
            Node<Room> current = rooms.First;
            while (current != null)
            {
                if (current.Data.IsAvailable)
                {
                    Console.WriteLine(current.Data);
                }
                current = current.Next;
            }
        }

        // Check-in a customer
        public void CheckIn(string roomNumber, string name)
        {
            Node<string> bookingNode = customerBookings.First;
            while (bookingNode != null)
            {
                if (bookingNode.Data == roomNumber)
                {
                    Node<Room> roomNode = rooms.First;
                    while (roomNode != null)
                    {
                        if (roomNode.Data.RoomNumber == roomNumber && !roomNode.Data.IsAvailable)
                        {
                            Console.WriteLine($"Room {roomNumber} checked in at {DateTime.Now}.");
                            return;
                        }
                        roomNode = roomNode.Next;
                    }
                    Console.WriteLine("Room is not booked.");
                    return;
                }
                bookingNode = bookingNode.Next;
            }
            Console.WriteLine($"Room {roomNumber} is not booked by {name}.");
        }

        // Checkout a customer
        public void CheckOut(string roomNumber, string name)
        {
            Node<string> bookingNode = customerBookings.First;
            while (bookingNode != null)
            {
                if (bookingNode.Data == roomNumber)
                {
                    Node<Room> roomNode = rooms.First;
                    while (roomNode != null)
                    {
                        if (roomNode.Data.RoomNumber == roomNumber && !roomNode.Data.IsAvailable)
                        {
                            roomNode.Data.IsAvailable = true;
                            Console.WriteLine($"Room {roomNumber} has been checked out at {DateTime.Now}.");

                            // Remove the customer from the customer list
                            Node<Customer> customerNode = customerManagement.Customers.First;
                            while (customerNode != null)
                            {
                                if (customerNode.Data.RoomNumber == roomNumber)
                                {
                                    customerManagement.Customers.Remove(customerNode);
                                    break;
                                }
                                customerNode = customerNode.Next;
                            }

                            customerBookings.Remove(bookingNode);
                            return;
                        }
                        roomNode = roomNode.Next;
                    }
                    Console.WriteLine("Room is not booked.");
                    return;
                }
                bookingNode = bookingNode.Next;
            }
            Console.WriteLine("You have not booked this room.");
        }

        // Start the booking process
        public void StartBookingProcess()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("           BOOKING MENU             ");
            Console.WriteLine("====================================");

            Console.Write("Enter your Name: ");
            string currentCustomerName = Console.ReadLine();
            Console.Write("Enter your Contact Number: ");
            string contactNumber = Console.ReadLine();

            Console.WriteLine($"Welcome To Hotel Trivago!");
            Console.Write("How many rooms would you like to book? ");

            int roomCount;
            while (!int.TryParse(Console.ReadLine(), out roomCount) || roomCount < 1 || roomCount > rooms.Count)
            {
                Console.WriteLine("Invalid number of rooms. Please try again.");
                Console.Write("How many rooms would you like to book? ");
            }

            int totalBill = 0;
            for (int i = 0; i < roomCount; i++)
            {
                Console.WriteLine("\nAvailable Rooms:");
                DisplayRooms();

                Console.Write("Enter Room Number to Book: ");
                string roomToBook = Console.ReadLine();
                Node<Room> roomNode = rooms.First;
                while (roomNode != null)
                {
                    if (roomNode.Data.RoomNumber == roomToBook && roomNode.Data.IsAvailable)
                    {
                        Console.Write("Enter Number of Nights: ");
                        int nights;
                        while (!int.TryParse(Console.ReadLine(), out nights) || nights < 1)
                        {
                            Console.WriteLine("Invalid input. Enter a valid number of nights.");
                            Console.Write("Enter Number of Nights: ");
                        }

                        roomNode.Data.IsAvailable = false;
                        customerBookings.AddLast(roomToBook);
                        totalBill += roomNode.Data.PricePerNight * nights;

                        // Add the customer to the customer list
                        customerManagement.AddCustomer(currentCustomerName, contactNumber, roomToBook);

                        Console.WriteLine($"Room {roomToBook} booked for {nights} nights.");
                        break;
                    }
                    roomNode = roomNode.Next;
                }

                if (roomNode == null)
                {
                    Console.WriteLine("Invalid or unavailable room. Please try again.");
                    i--; // Retry the booking for this room
                }
            }

            Console.WriteLine($"\nTotal Bill for {currentCustomerName}: ${totalBill}");
            Console.WriteLine("Thank you for using our Hotel Booking System. Goodbye!");
        }
    }

    // Person Class
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    // Employee Class
    public class Employee : Person
    {
        public string EmployeeGender { get; set; }
        public string EmployeePhoneNo { get; set; }
        public string EmployeeID { get; set; }
        public int EmployeePassword { get; set; }
        public bool IsAvailableNow { get; set; }

        public Employee(string name, int age, string employeeGender, string employeePhoneNo, string employeeID, bool isAvailableNow, int employeePassword)
            : base(name, age)
        {
            EmployeeGender = employeeGender;
            EmployeePhoneNo = employeePhoneNo;
            EmployeeID = employeeID;
            IsAvailableNow = isAvailableNow;
            EmployeePassword = employeePassword;
        }

        // Display Employee Info
        public void DisplayEmployeeInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Available: {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // SecurityGuard Class
    public class SecurityGuard : Employee
    {
        public int SecurityGuardPay { get; set; }

        public SecurityGuard(string name, int age, string gender, string phoneNo, string id, bool avail, int pass, int pay)
            : base(name, age, gender, phoneNo, id, avail, pass)
        {
            SecurityGuardPay = pay;
        }

        public void DisplaySGInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Salary: {SecurityGuardPay}, Available {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // HouseKeeping Class
    public class HouseKeeping : Employee
    {
        public int HouseKeepingPay { get; set; }

        public HouseKeeping(string name, int age, string gender, string phoneNo, string id, bool avail, int pass, int pay)
            : base(name, age, gender, phoneNo, id, avail, pass)
        {
            HouseKeepingPay = pay;
        }

        public void DisplayHKInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Salary: {HouseKeepingPay}, Available {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // EmployeeManagement Class
    public class EmployeeManagement
    {
        private CustomLinkedList<Employee> employees = new CustomLinkedList<Employee>();

        public Employee ValidateEmployee(string id, int password)
        {
            Node<Employee> current = employees.First;
            while (current != null)
            {
                if (current.Data.EmployeeID == id && current.Data.EmployeePassword == password)
                {
                    return current.Data; // Employee found and validated
                }
                current = current.Next;
            }
            return null; // Employee not found
        }

        public void ClockingIn(Employee employee)
        {
            Console.WriteLine($"Employee {employee.EmployeeID} has clocked in at {DateTime.Now}");
            System.Threading.Thread.Sleep(3000);
            employee.IsAvailableNow = true;
        }

        public void ClockingOut(Employee employee)
        {
            Console.WriteLine($"Employee {employee.EmployeeID} has clocked out at {DateTime.Now}");
            System.Threading.Thread.Sleep(3000);
            employee.IsAvailableNow = true;
        }

        public void AddEmployee(Employee employee)
        {
            employees.AddLast(employee);
            Console.WriteLine($"Employee {employee.Name} added.");
        }

        public void RemoveEmployee(string employeeId)
        {
            Node<Employee> current = employees.First;
            while (current != null)
            {
                if (current.Data.EmployeeID == employeeId)
                {
                    employees.Remove(current);
                    Console.WriteLine($"Employee {current.Data.Name} has been removed.");
                    return;
                }
                current = current.Next;
            }
            Console.WriteLine("Employee not found.");
        }

        // Insertion Sort for employees by name
        public void InsertionSortEmployees()
        {
            if (employees.First == null || employees.First.Next == null)
                return;

            Node<Employee> sorted = null; // Initialize sorted list
            Node<Employee> current = employees.First;

            while (current != null)
            {
                Node<Employee> next = current.Next; // Store the next node

                // Insert the current node into the sorted list
                if (sorted == null || CompareStringsLexicographically(sorted.Data.Name, current.Data.Name) >= 0)
                {
                    // Insert at the beginning
                    current.Next = sorted;
                    sorted = current;
                }
                else
                {
                    // Traverse the sorted list to find the correct position
                    Node<Employee> temp = sorted;
                    while (temp.Next != null && CompareStringsLexicographically(temp.Next.Data.Name, current.Data.Name) < 0)
                    {
                        temp = temp.Next;
                    }
                    current.Next = temp.Next;
                    temp.Next = current;
                }

                current = next; // Move to the next node
            }

            // Update the head and tail of the list
            employees.Head = sorted;
            employees.Tail = sorted;
            while (employees.Tail != null && employees.Tail.Next != null)
            {
                employees.Tail = employees.Tail.Next;
            }
        }

        private int CompareStringsLexicographically(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            int minLength = Math.Min(len1, len2);

            // Convert strings to lowercase for case-insensitive comparison
            str1 = str1.ToLower();
            str2 = str2.ToLower();

            for (int i = 0; i < minLength; i++)
            {
                if (str1[i] < str2[i])
                    return -1;
                if (str1[i] > str2[i])
                    return 1;
            }

            if (len1 < len2)
                return -1;
            if (len1 > len2)
                return 1;

            return 0;
        }

        public void DisplayEmployees()
        {
            InsertionSortEmployees(); // Ensure the list is sorted before displaying
            Node<Employee> current = employees.First;
            while (current != null)
            {
                current.Data.DisplayEmployeeInfo();
                current = current.Next;
            }
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize HotelManagement, EmployeeManagement, and CustomerManagement objects
            CustomerManagement customerManagement = new CustomerManagement();
            HotelManagement hotelManagement = new HotelManagement(customerManagement);
            EmployeeManagement employeeManagement = new EmployeeManagement();

            // Add sample employees
            employeeManagement.AddEmployee(new SecurityGuard("John Doe", 30, "Male", "1234567890", "SG001", false, 123, 300));
            employeeManagement.AddEmployee(new HouseKeeping("Fang Yuan", 22, "Male", "1234567360", "HK001", false, 123, 400));

            // Add dummy customers who have already booked rooms
            customerManagement.AddCustomer("Bob", "2222222222", "002");
            customerManagement.AddCustomer("Charlie", "3333333333", "104");

            while (true) // Keep the main menu active until exit
            {
                Console.Clear(); // Clears the screen before displaying the main menu
                Console.WriteLine("====================================");
                Console.WriteLine("       WELCOME TO HOTEL TRIVAGO     ");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("Press C if you are a CUSTOMER");
                Console.WriteLine("Press E if you are an EMPLOYEE");
                Console.WriteLine("Press X to EXIT");
                string h = Utility.GetNonEmptyString("");

                if (h == "c" || h == "C")
                {
                    bool usingService = true;
                    while (usingService)
                    {
                        Console.Clear(); // Clears screen before displaying customer menu
                        Console.WriteLine("====================================");
                        Console.WriteLine("           CUSTOMER MENU           ");
                        Console.WriteLine("====================================");
                        Console.WriteLine("Booking --- A");
                        Console.WriteLine("Check In --- B");
                        Console.WriteLine("Check Out --- C");
                        Console.WriteLine("Back to Main Menu --- X");
                        string s = Console.ReadLine();

                        if (s == "a" || s == "A")
                        {
                            hotelManagement.StartBookingProcess();
                        }
                        else if (s == "b" || s == "B")
                        {
                            Console.Clear();
                            Console.WriteLine("====================================");
                            Console.WriteLine("           CHECK-IN MENU           ");
                            Console.WriteLine("====================================");
                            Console.WriteLine("Enter your name:");
                            string Name = Console.ReadLine();
                            Console.WriteLine("Enter the Room Number:");
                            string RoomNo = Console.ReadLine();
                            hotelManagement.CheckIn(RoomNo, Name);
                            Console.WriteLine("Have a pleasant stay");
                        }
                        else if (s == "c" || s == "C")
                        {
                            Console.Clear();
                            Console.WriteLine("====================================");
                            Console.WriteLine("          CHECK-OUT MENU           ");
                            Console.WriteLine("====================================");
                            Console.WriteLine("Enter your name:");
                            string Name = Console.ReadLine();
                            Console.WriteLine("Enter the Room Number:");
                            string RoomNo = Console.ReadLine();
                            hotelManagement.CheckOut(RoomNo, Name);
                            Console.WriteLine("Thank you for staying with us");
                        }
                        else if (s == "x" || s == "X")
                        {
                            break; // Return to the main menu
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                        }

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(); // Pause before clearing the screen
                    }
                }
                else if (h == "e" || h == "E")
                {
                    Console.Clear();
                    int mP = 123; // Manager password

                    Console.WriteLine("====================================");
                    Console.WriteLine("           EMPLOYEE MENU           ");
                    Console.WriteLine("====================================");
                    Console.WriteLine("S for Staff");
                    Console.WriteLine("M for Manager");
                    Console.WriteLine("Back to Main Menu --- X");
                    string v = Utility.GetNonEmptyString("");

                    if (v == "s" || v == "S")
                    {

                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("           STAFF MENU             ");
                        Console.WriteLine("====================================");

                        // Use Utility.GetNonEmptyString for Employee ID input
                        string g = Utility.GetNonEmptyString("Enter ID: ");

                        // Use Utility.GetIntInput for Password input
                        int password = Utility.GetIntInput("Enter Password: ");

                        // Validate the employee
                        Employee validatedEmployee = employeeManagement.ValidateEmployee(g, password);

                        if (validatedEmployee != null)
                        {
                            Console.WriteLine($"Employee {validatedEmployee.Name} validated successfully.");
                            Console.WriteLine($"ID: {validatedEmployee.EmployeeID}, Gender: {validatedEmployee.EmployeeGender}");

                            // Use Utility.GetNonEmptyString for menu selection
                            string t = Utility.GetNonEmptyString("Press I ----> Clock in\nPress O ----> Clock out\nBack to Main Menu --- X\n");

                            if (t == "i" || t == "I")
                            {
                                employeeManagement.ClockingIn(validatedEmployee);
                            }
                            else if (t == "o" || t == "O")
                            {
                                employeeManagement.ClockingOut(validatedEmployee);
                            }
                            else if (t == "x" || t == "X")
                            {
                                // Return to the main menu
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID or Password.");
                            Thread.Sleep(3000);
                        }
                    }
                    else if (v == "m" || v == "M")
                    {

                        while (true)
                        {
                            Console.Write("Enter Password: ");
                            string managerPasswordInput = Console.ReadLine();
                            if (int.TryParse(managerPasswordInput, out int p) && p == mP)
                            {


                                while (true)
                                {

                                    Console.Clear();
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("           MANAGER MENU           ");
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("S ----> Staff");
                                    Console.WriteLine("C ----> Customers");
                                    Console.WriteLine("Back to Main Menu --- X");
                                    string d = Console.ReadLine();

                                    if (d == "s" || d == "S")
                                    {
                                        while (true)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("====================================");
                                            Console.WriteLine("           STAFF MANAGEMENT         ");
                                            Console.WriteLine("====================================");
                                            Console.WriteLine("V             ----> View Employees");
                                            Console.WriteLine("F             ----> Fire Employees");
                                            Console.WriteLine("A             ----> Add Employees");
                                            Console.WriteLine("Back to Main Menu --- X");
                                            string y = Console.ReadLine();

                                            if (y == "v" || y == "V")
                                            {
                                                employeeManagement.InsertionSortEmployees(); // Sort employees before displaying
                                                employeeManagement.DisplayEmployees();
                                                Console.WriteLine("Press any key to continue...");
                                                Console.ReadKey();
                                            }
                                            else if (y == "A" || y == "a")
                                            {
                                                while (true)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("====================================");
                                                    Console.WriteLine("           ADD EMPLOYEE            ");
                                                    Console.WriteLine("====================================");
                                                    Console.WriteLine("H ----> House Keeping");
                                                    Console.WriteLine("S ----> Security Guard");
                                                    Console.WriteLine("Back to Main Menu --- X");
                                                    string w = Console.ReadLine();

                                                    if (w == "h" || w == "H")
                                                    {
                                                        string name = Utility.GetNonEmptyString("Name: ");
                                                        int age = Utility.GetIntInput("Age: ");
                                                        string gen = Utility.GetNonEmptyString("Gender: ");
                                                        string phoneno = Utility.GetNonEmptyString("Phone No: ");
                                                        string id = Utility.GetNonEmptyString("Employee ID: ");
                                                        bool av = Utility.GetBoolInput("Availability at the moment: ");
                                                        int pw = Utility.GetIntInput("Employee Password: ");
                                                        int payyy = Utility.GetIntInput("Employee Pay: ");


                                                        employeeManagement.AddEmployee(new HouseKeeping(name, age, gen, phoneno, id, av, pw, payyy));
                                                        Thread.Sleep(3000);

                                                    }
                                                    else if (w == "s" || w == "S")
                                                    {
                                                        string ame = Utility.GetNonEmptyString("Name: ");
                                                        int ge = Utility.GetIntInput("Age: ");
                                                        string en = Utility.GetNonEmptyString("Gender: ");
                                                        string honeno = Utility.GetNonEmptyString("Phone No: ");
                                                        string ida = Utility.GetNonEmptyString("Employee ID: ");
                                                        bool ava = Utility.GetBoolInput("Availability at the moment: ");
                                                        int pwa = Utility.GetIntInput("Employee Password: ");
                                                        int payyyy = Utility.GetIntInput("Employee Pay: ");

                                                        employeeManagement.AddEmployee(new SecurityGuard(ame, ge, en, honeno, ida, ava, pwa, payyyy));
                                                        Thread.Sleep(3000);

                                                    }
                                                    else if (w == "x" || w == "X")
                                                    {
                                                        break; // Return to the main menu
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Invalid input. Please try again.");
                                                    }
                                                }

                                            }
                                            else if (y == "f" || y == "F")
                                            {
                                                while (true)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("====================================");
                                                    Console.WriteLine("           FIRE EMPLOYEE           ");
                                                    Console.WriteLine("====================================");
                                                    Console.WriteLine("H ----> House Keeping");
                                                    Console.WriteLine("S ----> Security Guard");
                                                    Console.WriteLine("Back to Main Menu --- X");
                                                    string w = Console.ReadLine();

                                                    if (w == "h" || w == "H")
                                                    {
                                                        Console.WriteLine("ID of Employee to be Fired: ");
                                                        string k = Console.ReadLine();
                                                        employeeManagement.RemoveEmployee(k);
                                                        Thread.Sleep(3000);
                                                    }
                                                    else if (w == "s" || w == "S")
                                                    {
                                                        Console.WriteLine("ID of Employee to be Fired: ");
                                                        string k = Console.ReadLine();
                                                        employeeManagement.RemoveEmployee(k);
                                                        Thread.Sleep(3000);
                                                    }
                                                    else if (w == "x" || w == "X")
                                                    {
                                                        break; // Return to the main menu
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Invalid input. Please try again.");
                                                    }
                                                }

                                            }
                                            else if (y == "x" || y == "X")
                                            {
                                                break; // Return to the main menu
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid input. Please try again.");
                                            }
                                        }

                                    }
                                    else if (d == "c" || d == "C")
                                    {
                                        Console.Clear();
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("           CUSTOMER LIST           ");
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("List of Customers (Sorted by Name):");
                                        customerManagement.DisplayCustomers();
                                        Console.WriteLine("Press any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                    else if (d == "x" || d == "X")
                                    {
                                        break; // Return to the main menu
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Please try again.");
                                        Thread.Sleep(2000);

                                    }
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Password. Please Try Again.");


                            }
                        }


                    }
                    else if (v == "x" || v == "X")
                    {
                        // Return to the main menu
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
                else if (h == "x" || h == "X")
                {
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("  THANK YOU FOR USING HOTEL TRIVAGO");
                    Console.WriteLine("====================================");
                    break; // Exit the program
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }
    }
}

/*
Here we used Merge Sort for Customers and Bubble Sort for Employees

using System;

namespace HotelManagementSystem
{
    // Custom LinkedList Node
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    // Custom LinkedList
    public class CustomLinkedList<T>
    {
        public Node<T> Head { get; set; }
        public Node<T> Tail { get; set; }
        public int Count { get; set; }

        public CustomLinkedList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public void AddLast(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                Tail = newNode;
            }
            Count++;
        }

        public void Clear()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public void Remove(Node<T> node)
        {
            if (node == null || Head == null)
                return;

            if (node == Head)
            {
                Head = Head.Next;
                if (Head == null)
                    Tail = null;
                Count--;
                return;
            }

            Node<T> current = Head;
            while (current.Next != null)
            {
                if (current.Next == node)
                {
                    current.Next = node.Next;
                    if (node == Tail)
                        Tail = current;
                    Count--;
                    return;
                }
                current = current.Next;
            }
        }

        public Node<T> First => Head;
    }

    // Customer Class
    public class Customer
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string RoomNumber { get; set; }

        public Customer(string name, string contactNumber, string roomNumber)
        {
            Name = name;
            ContactNumber = contactNumber;
            RoomNumber = roomNumber;
        }

        public override string ToString()
        {
            return $"Customer: {Name}, Contact: {ContactNumber}, Room: {RoomNumber}";
        }
    }

    // CustomerManagement Class
    public class CustomerManagement
    {
        private CustomLinkedList<Customer> customers = new CustomLinkedList<Customer>();

        // Add a customer and sort the list
        public void AddCustomer(string name, string contactNumber, string roomNumber)
        {
            customers.AddLast(new Customer(name, contactNumber, roomNumber));
            MergeSortCustomers(); // Sort the list after adding a new customer
        }

        // Display all customers
        public void DisplayCustomers()
        {
            Node<Customer> current = customers.First;
            while (current != null)
            {
                Console.WriteLine(current.Data);
                current = current.Next;
            }
        }

        // Merge Sort for customers by name
        public void MergeSortCustomers()
        {
            customers.Head = MergeSort(customers.First);
            Node<Customer> current = customers.First;
            while (current != null && current.Next != null)
            {
                current = current.Next;
            }
            customers.Tail = current;
        }

        private Node<Customer> MergeSort(Node<Customer> head)
        {
            if (head == null || head.Next == null)
                return head;

            Node<Customer> middle = GetMiddle(head);
            Node<Customer> nextOfMiddle = middle.Next;
            middle.Next = null;

            Node<Customer> left = MergeSort(head);
            Node<Customer> right = MergeSort(nextOfMiddle);

            return Merge(left, right);
        }

        private Node<Customer> Merge(Node<Customer> left, Node<Customer> right)
        {
            Node<Customer> result = null;

            if (left == null)
                return right;
            if (right == null)
                return left;

            if (CompareStringsLexicographically(left.Data.Name, right.Data.Name) <= 0)
            {
                result = left;
                result.Next = Merge(left.Next, right);
            }
            else
            {
                result = right;
                result.Next = Merge(left, right.Next);
            }

            return result;
        }

        private Node<Customer> GetMiddle(Node<Customer> head)
        {
            if (head == null)
                return head;

            Node<Customer> slow = head;
            Node<Customer> fast = head;

            while (fast.Next != null && fast.Next.Next != null)
            {
                slow = slow.Next;
                fast = fast.Next.Next;
            }

            return slow;
        }

        private int CompareStringsLexicographically(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            int minLength = Math.Min(len1, len2);

            for (int i = 0; i < minLength; i++)
            {
                if (str1[i] < str2[i])
                    return -1;
                if (str1[i] > str2[i])
                    return 1;
            }

            if (len1 < len2)
                return -1;
            if (len1 > len2)
                return 1;

            return 0;
        }
    }

    // Room Class
    public class Room
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; } // Luxury or Normal
        public bool IsAvailable { get; set; }
        public int PricePerNight { get; set; }

        public Room(string roomNumber, string type, bool isAvailable, int price)
        {
            RoomNumber = roomNumber;
            Type = type;
            IsAvailable = isAvailable;
            PricePerNight = price;
        }

        public override string ToString()
        {
            return $"Room {RoomNumber}: {Type} - {(IsAvailable ? "Available" : "Booked")} - ${PricePerNight}/night";
        }
    }

    // HotelManagement Class
    public class HotelManagement
    {
        private CustomLinkedList<Room> rooms = new CustomLinkedList<Room>();
        private CustomLinkedList<string> customerBookings = new CustomLinkedList<string>(); // RoomNumber → CustomerName mapping
        private CustomerManagement customerManagement;

        public HotelManagement(CustomerManagement customerManagement)
        {
            this.customerManagement = customerManagement;

            // Add rooms only once during initialization
            AddRoom("001", "Luxury Suite", false, 300); // Already booked
            AddRoom("002", "Luxury Suite", false, 300); // Already booked
            AddRoom("003", "Normal Room", true, 100);
            AddRoom("004", "Normal Room", true, 100);
            AddRoom("005", "Normal Room", true, 100);
            AddRoom("101", "Luxury Suite", true, 300);
            AddRoom("102", "Luxury Suite", true, 300);
            AddRoom("103", "Normal Room", true, 100);
            AddRoom("104", "Normal Room", false, 100); // Already booked
            AddRoom("105", "Normal Room", true, 100);
        }

        // Add a room dynamically
        public void AddRoom(string roomNumber, string type, bool isAvailable, int price)
        {
            rooms.AddLast(new Room(roomNumber, type, isAvailable, price));
        }

        // Display available rooms
        public void DisplayRooms()
        {
            Node<Room> current = rooms.First;
            while (current != null)
            {
                if (current.Data.IsAvailable)
                {
                    Console.WriteLine(current.Data);
                }
                current = current.Next;
            }
        }

        // Check-in a customer
        public void CheckIn(string roomNumber, string name)
        {
            Node<string> bookingNode = customerBookings.First;
            while (bookingNode != null)
            {
                if (bookingNode.Data == roomNumber)
                {
                    Node<Room> roomNode = rooms.First;
                    while (roomNode != null)
                    {
                        if (roomNode.Data.RoomNumber == roomNumber && !roomNode.Data.IsAvailable)
                        {
                            Console.WriteLine($"Room {roomNumber} checked in at {DateTime.Now}.");
                            return;
                        }
                        roomNode = roomNode.Next;
                    }
                    Console.WriteLine("Room is not booked.");
                    return;
                }
                bookingNode = bookingNode.Next;
            }
            Console.WriteLine($"Room {roomNumber} is not booked by {name}.");
        }

        // Checkout a customer
        public void CheckOut(string roomNumber, string name)
        {
            Node<string> bookingNode = customerBookings.First;
            while (bookingNode != null)
            {
                if (bookingNode.Data == roomNumber)
                {
                    Node<Room> roomNode = rooms.First;
                    while (roomNode != null)
                    {
                        if (roomNode.Data.RoomNumber == roomNumber && !roomNode.Data.IsAvailable)
                        {
                            roomNode.Data.IsAvailable = true;
                            Console.WriteLine($"Room {roomNumber} has been checked out at {DateTime.Now}.");
                            customerBookings.Remove(bookingNode);
                            return;
                        }
                        roomNode = roomNode.Next;
                    }
                    Console.WriteLine("Room is not booked.");
                    return;
                }
                bookingNode = bookingNode.Next;
            }
            Console.WriteLine("You have not booked this room.");
        }

        // Start the booking process
        public void StartBookingProcess()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("           BOOKING MENU             ");
            Console.WriteLine("====================================");

            Console.Write("Enter your Name: ");
            string currentCustomerName = Console.ReadLine();
            Console.Write("Enter your Contact Number: ");
            string contactNumber = Console.ReadLine();

            Console.WriteLine($"Welcome To Hotel Trivago!");
            Console.Write("How many rooms would you like to book? ");

            int roomCount;
            while (!int.TryParse(Console.ReadLine(), out roomCount) || roomCount < 1 || roomCount > rooms.Count)
            {
                Console.WriteLine("Invalid number of rooms. Please try again.");
                Console.Write("How many rooms would you like to book? ");
            }

            int totalBill = 0;
            for (int i = 0; i < roomCount; i++)
            {
                Console.WriteLine("\nAvailable Rooms:");
                DisplayRooms();

                Console.Write("Enter Room Number to Book: ");
                string roomToBook = Console.ReadLine();
                Node<Room> roomNode = rooms.First;
                while (roomNode != null)
                {
                    if (roomNode.Data.RoomNumber == roomToBook && roomNode.Data.IsAvailable)
                    {
                        Console.Write("Enter Number of Nights: ");
                        int nights;
                        while (!int.TryParse(Console.ReadLine(), out nights) || nights < 1)
                        {
                            Console.WriteLine("Invalid input. Enter a valid number of nights.");
                            Console.Write("Enter Number of Nights: ");
                        }

                        roomNode.Data.IsAvailable = false;
                        customerBookings.AddLast(roomToBook);
                        totalBill += roomNode.Data.PricePerNight * nights;

                        // Add the customer to the customer list
                        customerManagement.AddCustomer(currentCustomerName, contactNumber, roomToBook);

                        Console.WriteLine($"Room {roomToBook} booked for {nights} nights.");
                        break;
                    }
                    roomNode = roomNode.Next;
                }

                if (roomNode == null)
                {
                    Console.WriteLine("Invalid or unavailable room. Please try again.");
                    i--; // Retry the booking for this room
                }
            }

            Console.WriteLine($"\nTotal Bill for {currentCustomerName}: ${totalBill}");
            Console.WriteLine("Thank you for using our Hotel Booking System. Goodbye!");
        }
    }

    // Person Class
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    // Employee Class
    public class Employee : Person
    {
        public string EmployeeGender { get; set; }
        public string EmployeePhoneNo { get; set; }
        public string EmployeeID { get; set; }
        public int EmployeePassword { get; set; }
        public bool IsAvailableNow { get; set; }

        public Employee(string name, int age, string employeeGender, string employeePhoneNo, string employeeID, bool isAvailableNow, int employeePassword)
            : base(name, age)
        {
            EmployeeGender = employeeGender;
            EmployeePhoneNo = employeePhoneNo;
            EmployeeID = employeeID;
            IsAvailableNow = isAvailableNow;
            EmployeePassword = employeePassword;
        }

        // Display Employee Info
        public void DisplayEmployeeInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Available: {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // SecurityGuard Class
    public class SecurityGuard : Employee
    {
        public int SecurityGuardPay { get; set; }

        public SecurityGuard(string name, int age, string gender, string phoneNo, string id, bool avail, int pass, int pay)
            : base(name, age, gender, phoneNo, id, avail, pass)
        {
            SecurityGuardPay = pay;
        }

        public void DisplaySGInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Salary: {SecurityGuardPay}, Available {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // HouseKeeping Class
    public class HouseKeeping : Employee
    {
        public int HouseKeepingPay { get; set; }

        public HouseKeeping(string name, int age, string gender, string phoneNo, string id, bool avail, int pass, int pay)
            : base(name, age, gender, phoneNo, id, avail, pass)
        {
            HouseKeepingPay = pay;
        }

        public void DisplayHKInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Gender: {EmployeeGender}, Phone: {EmployeePhoneNo}, ID: {EmployeeID}, Salary: {HouseKeepingPay}, Available {IsAvailableNow}, Employee Password: {EmployeePassword}");
        }
    }

    // EmployeeManagement Class
    public class EmployeeManagement
    {
        private CustomLinkedList<Employee> employees = new CustomLinkedList<Employee>();

        public Employee ValidateEmployee(string id, int password)
        {
            Node<Employee> current = employees.First;
            while (current != null)
            {
                if (current.Data.EmployeeID == id && current.Data.EmployeePassword == password)
                {
                    return current.Data; // Employee found and validated
                }
                current = current.Next;
            }
            return null; // Employee not found
        }

        public void ClockingIn(Employee employee)
        {
            Console.WriteLine($"Employee {employee.EmployeeID} has clocked in at {DateTime.Now}");
            System.Threading.Thread.Sleep(3000);
            employee.IsAvailableNow = true;
        }

        public void ClockingOut(Employee employee)
        {
            Console.WriteLine($"Employee {employee.EmployeeID} has clocked out at {DateTime.Now}");
            System.Threading.Thread.Sleep(3000);
            employee.IsAvailableNow = true;
        }

        public void AddEmployee(Employee employee)
        {
            employees.AddLast(employee);
            Console.WriteLine($"Employee {employee.Name} added.");
        }

        public void RemoveEmployee(string employeeId)
        {
            Node<Employee> current = employees.First;
            while (current != null)
            {
                if (current.Data.EmployeeID == employeeId)
                {
                    employees.Remove(current);
                    Console.WriteLine($"Employee {current.Data.Name} has been removed.");
                    return;
                }
                current = current.Next;
            }
            Console.WriteLine("Employee not found.");
        }

        // Bubble Sort for employees by name
        public void BubbleSortEmployees()
        {
            Node<Employee> current = employees.First;
            Node<Employee> index = null;
            Employee temp;

            if (current == null)
                return;

            while (current != null)
            {
                index = current.Next;
                while (index != null)
                {
                    if (CompareStringsLexicographically(current.Data.Name, index.Data.Name) > 0)
                    {
                        temp = current.Data;
                        current.Data = index.Data;
                        index.Data = temp;
                    }
                    index = index.Next;
                }
                current = current.Next;
            }
        }

        private int CompareStringsLexicographically(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            int minLength = Math.Min(len1, len2);

            for (int i = 0; i < minLength; i++)
            {
                if (str1[i] < str2[i])
                    return -1;
                if (str1[i] > str2[i])
                    return 1;
            }

            if (len1 < len2)
                return -1;
            if (len1 > len2)
                return 1;

            return 0;
        }

        public void DisplayEmployees()
        {
            Node<Employee> current = employees.First;
            while (current != null)
            {
                current.Data.DisplayEmployeeInfo();
                current = current.Next;
            }
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize HotelManagement, EmployeeManagement, and CustomerManagement objects
            CustomerManagement customerManagement = new CustomerManagement();
            HotelManagement hotelManagement = new HotelManagement(customerManagement);
            EmployeeManagement employeeManagement = new EmployeeManagement();

            // Add sample employees
            employeeManagement.AddEmployee(new SecurityGuard("John Doe", 30, "Male", "1234567890", "SG001", false, 12345, 300));
            employeeManagement.AddEmployee(new HouseKeeping("Fang Yuan", 22, "Male", "1234567360", "HK001", false, 22345, 400));

            // Add dummy customers who have already booked rooms
            customerManagement.AddCustomer("Bob", "2222222222", "002");
            customerManagement.AddCustomer("Charlie", "3333333333", "104");

            while (true) // Keep the main menu active until exit
            {
                Console.Clear(); // Clears the screen before displaying the main menu
                Console.WriteLine("====================================");
                Console.WriteLine("       WELCOME TO HOTEL TRIVAGO     ");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("Press C if you are a CUSTOMER");
                Console.WriteLine("Press E if you are an EMPLOYEE");
                Console.WriteLine("Press X to EXIT");
                string h = Console.ReadLine();

                if (h == "c" || h == "C")
                {
                    bool usingService = true;
                    while (usingService)
                    {
                        Console.Clear(); // Clears screen before displaying customer menu
                        Console.WriteLine("====================================");
                        Console.WriteLine("           CUSTOMER MENU           ");
                        Console.WriteLine("====================================");
                        Console.WriteLine("Booking --- A");
                        Console.WriteLine("Check In --- B");
                        Console.WriteLine("Check Out --- C");
                        Console.WriteLine("Back to Main Menu --- X");
                        string s = Console.ReadLine();

                        if (s == "a" || s == "A")
                        {
                            hotelManagement.StartBookingProcess();
                        }
                        else if (s == "b" || s == "B")
                        {
                            Console.Clear();
                            Console.WriteLine("====================================");
                            Console.WriteLine("           CHECK-IN MENU           ");
                            Console.WriteLine("====================================");
                            Console.WriteLine("Enter your name:");
                            string Name = Console.ReadLine();
                            Console.WriteLine("Enter the Room Number:");
                            string RoomNo = Console.ReadLine();
                            hotelManagement.CheckIn(RoomNo, Name);
                            Console.WriteLine("Have a pleasant stay");
                        }
                        else if (s == "c" || s == "C")
                        {
                            Console.Clear();
                            Console.WriteLine("====================================");
                            Console.WriteLine("          CHECK-OUT MENU           ");
                            Console.WriteLine("====================================");
                            Console.WriteLine("Enter your name:");
                            string Name = Console.ReadLine();
                            Console.WriteLine("Enter the Room Number:");
                            string RoomNo = Console.ReadLine();
                            hotelManagement.CheckOut(RoomNo, Name);
                            Console.WriteLine("Thank you for staying with us");
                        }
                        else if (s == "x" || s == "X")
                        {
                            break; // Return to the main menu
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                        }

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(); // Pause before clearing the screen
                    }
                }
                else if (h == "e" || h == "E")
                {
                    Console.Clear();
                    int mP = 123; // Manager password

                    Console.WriteLine("====================================");
                    Console.WriteLine("           EMPLOYEE MENU           ");
                    Console.WriteLine("====================================");
                    Console.WriteLine("S for Staff");
                    Console.WriteLine("M for Manager");
                    Console.WriteLine("Back to Main Menu --- X");
                    string v = Console.ReadLine();

                    if (v == "s" || v == "S")
                    {
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("           STAFF MENU             ");
                        Console.WriteLine("====================================");
                        Console.WriteLine("Enter ID:");
                        string g = Console.ReadLine(); // Employee ID input

                        Console.WriteLine("Enter Password:");
                        string inputPassword = Console.ReadLine(); // Password input as string

                        if (int.TryParse(inputPassword, out int password))
                        {
                            Employee validatedEmployee = employeeManagement.ValidateEmployee(g, password);

                            if (validatedEmployee != null)
                            {
                                Console.WriteLine($"Employee {validatedEmployee.Name} validated successfully.");
                                Console.WriteLine($"ID: {validatedEmployee.EmployeeID}, Gender: {validatedEmployee.EmployeeGender}");

                                Console.WriteLine("Press I ----> Clock in");
                                Console.WriteLine("Press O ----> Clock out");
                                Console.WriteLine("Back to Main Menu --- X");
                                string t = Console.ReadLine();

                                if (t == "i" || t == "I")
                                {
                                    employeeManagement.ClockingIn(validatedEmployee);
                                }
                                else if (t == "o" || t == "O")
                                {
                                    employeeManagement.ClockingOut(validatedEmployee);
                                }
                                else if (t == "x" || t == "X")
                                {
                                    break; // Return to the main menu
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID or Password.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID or password. Please try again.");
                            Thread.Sleep(3000);
                        }
                    }
                    else if (v == "m" || v == "M")
                    {
                        Console.Write("Enter Password: ");
                        string managerPasswordInput = Console.ReadLine();

                        if (int.TryParse(managerPasswordInput, out int p) && p == mP)
                        {
                            Console.Clear();
                            Console.WriteLine("====================================");
                            Console.WriteLine("           MANAGER MENU           ");
                            Console.WriteLine("====================================");
                            Console.WriteLine("S ----> Staff");
                            Console.WriteLine("C ----> Customers");
                            Console.WriteLine("Back to Main Menu --- X");
                            string d = Console.ReadLine();

                            while (true)
                            {
                                if (d == "s" || d == "S")
                                {
                                    Console.Clear();
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("           STAFF MANAGEMENT         ");
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("V             ----> View Employees");
                                    Console.WriteLine("F             ----> Fire Employees");
                                    Console.WriteLine("A             ----> Add Employees");
                                    Console.WriteLine("Back to Main Menu --- X");
                                    string y = Console.ReadLine();

                                    if (y == "v" || y == "V")
                                    {
                                        employeeManagement.BubbleSortEmployees(); // Sort employees before displaying
                                        employeeManagement.DisplayEmployees();
                                        Console.WriteLine("Press any key to continue...");
                                        Console.ReadKey();
                                    }
                                    else if (y == "A" || y == "a")
                                    {
                                        Console.Clear();
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("           ADD EMPLOYEE            ");
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("H ----> House Keeping");
                                        Console.WriteLine("S ----> Security Guard");
                                        Console.WriteLine("Back to Main Menu --- X");
                                        string w = Console.ReadLine();

                                        if (w == "h" || w == "H")
                                        {
                                            Console.WriteLine("Name: "); string name = Console.ReadLine();
                                            Console.WriteLine("Age: "); int age = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Gender: "); string gen = Console.ReadLine();
                                            Console.WriteLine("Phone No: "); string phoneno = Console.ReadLine();
                                            Console.WriteLine("Employee ID: "); string id = Console.ReadLine();
                                            Console.WriteLine("Availability at the moment: "); bool av = Convert.ToBoolean(Console.ReadLine());
                                            Console.WriteLine("Employee Password: "); int pw = Convert.ToInt32(Console.ReadLine());
                                            Console.WriteLine("Employee Pay: "); int payyy = Convert.ToInt32(Console.ReadLine());

                                            employeeManagement.AddEmployee(new HouseKeeping(name, age, gen, phoneno, id, av, pw, payyy));
                                        }
                                        else if (w == "s" || w == "S")
                                        {
                                            Console.WriteLine("Name: "); string ame = Console.ReadLine();
                                            Console.WriteLine("Age: "); int ge = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Gender: "); string en = Console.ReadLine();
                                            Console.WriteLine("Phone No: "); string honeno = Console.ReadLine();
                                            Console.WriteLine("Employee ID: "); string ida = Console.ReadLine();
                                            Console.WriteLine("Availability at the moment: "); bool ava = Convert.ToBoolean(Console.ReadLine());
                                            Console.WriteLine("Employee Password: "); int pwa = Convert.ToInt32(Console.ReadLine());
                                            Console.WriteLine("Employee Pay: "); int payyyy = Convert.ToInt32(Console.ReadLine());

                                            employeeManagement.AddEmployee(new SecurityGuard(ame, ge, en, honeno, ida, ava, pwa, payyyy));
                                        }
                                        else if (w == "x" || w == "X")
                                        {
                                            break; // Return to the main menu
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid input. Please try again.");
                                        }
                                    }
                                    else if (y == "f" || y == "F")
                                    {
                                        Console.Clear();
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("           FIRE EMPLOYEE           ");
                                        Console.WriteLine("====================================");
                                        Console.WriteLine("H ----> House Keeping");
                                        Console.WriteLine("S ----> Security Guard");
                                        Console.WriteLine("Back to Main Menu --- X");
                                        string w = Console.ReadLine();

                                        if (w == "h" || w == "H")
                                        {
                                            Console.WriteLine("ID of Employee to be Fired: ");
                                            string k = Console.ReadLine();
                                            employeeManagement.RemoveEmployee(k);
                                        }
                                        else if (w == "s" || w == "S")
                                        {
                                            Console.WriteLine("ID of Employee to be Fired: ");
                                            string k = Console.ReadLine();
                                            employeeManagement.RemoveEmployee(k);
                                        }
                                        else if (w == "x" || w == "X")
                                        {
                                            break; // Return to the main menu
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid input. Please try again.");
                                        }
                                    }
                                    else if (y == "x" || y == "X")
                                    {
                                        break; // Return to the main menu
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Please try again.");
                                    }
                                }
                                else if (d == "c" || d == "C")
                                {
                                    Console.Clear();
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("           CUSTOMER LIST           ");
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("List of Customers (Sorted by Name):");
                                    customerManagement.DisplayCustomers();
                                    Console.WriteLine("Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                else if (d == "x" || d == "X")
                                {
                                    break; // Return to the main menu
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Password. Please Try Again.");
                        }
                    }
                    else if (v == "x" || v == "X")
                    {
                        // Return to the main menu
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
                else if (h == "x" || h == "X")
                {
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("  THANK YOU FOR USING HOTEL TRIVAGO");
                    Console.WriteLine("====================================");
                    break; // Exit the program
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }
    }
}

*/