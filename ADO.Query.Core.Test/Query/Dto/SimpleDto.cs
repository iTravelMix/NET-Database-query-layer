namespace ADO.Query.Test.Query.Dto
{
    using System.Collections.Generic;

    public class SimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<PhoneDto> Phones { get; set; } 
    }

    public class NameValue
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<NameValue> Emails { get; set; }
        public IEnumerable<NameValue> Phones { get; set;  }
    }
}
