using OrderTracking.Domain;

namespace OrderTracking.UnitTests;

public class CustomerTests
{

    

    //customer with name guest
    [Fact]
    public void Customer_ShouldHaveCorrectName_WhenNameSetAsGuest()
    {
       Customer cus = Customer.CreateGuest();

       var result = cus.Name;
       Assert.Equal("Guest", result);
    }

    //customer with no email


    [Fact]
    public void Customer_ShouldHaveNullEmail_WhenEmailSetNull()
    {
       Customer cus = Customer.CreateGuest();

       var result = cus.Email;

       Assert.Null(result);
    
    }


    //2 customer produces diff ids

     [Fact]
    public void Customer_ShouldHaveTwoSeperateId_WhenTwoCustomerGenerated()
    {
       Customer cus1 = Customer.CreateGuest();
       Customer cus2 = Customer.CreateGuest();

       Assert.NotEqual(cus1.Id,cus2.Id);
    
    }



}