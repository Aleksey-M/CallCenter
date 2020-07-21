using System.ComponentModel.DataAnnotations;

namespace CallCenter.Model
{
    public class PersonsFilterFields
    {
        [Required, Range(1, 1000)]
        public int PageNo { get; set; } = 1;
        [Required, Range(10, 30)]
        public int PageSize { get; set; } = 25;
        [Required]
        public Gender Gender { get; set; } = Gender.All;
        public string NameFilter { get; set; } = string.Empty;
        public int? MaxAge { get; set; } = null;
        public int? MinAge { get; set; } = null;
        public int MinDaysAfterLastCall { get; set; } = 0;
        public override string ToString()
        {
            return $"Filter fields: PageNo:{PageNo}; PageSize:{PageSize}; Gender:{Gender}; NameFilter:{NameFilter}; MaxAge:{MaxAge}; MinAge:{MinAge}; MinDaysAfterLastCall:{MinDaysAfterLastCall}";
        }

        public string ToParams() =>
            $@"?pageno={PageNo}&pagesize={PageSize}&gender={Gender}&namefilter={NameFilter}&maxage={MaxAge}&minage={MinAge}&mindaysafterlastcall={MinDaysAfterLastCall}";
        
        public static PersonsFilterFields Default => new PersonsFilterFields();
    }
}
