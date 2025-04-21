using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public IndexModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        public List<ViewCustomerViewModel> Customers { get; set; }

        public string? SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string sortColumn,string sortOrder, string? searchTerm, int pageNo = 1)
        {
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            CurrentPage = pageNo;

            var dtos = await _customerService.GetViewAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dtos = dtos
                    .Where(x =>
                        x.Givenname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        x.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        x.CustomerId.ToString().Contains(searchTerm) ||
                        (!string.IsNullOrEmpty(x.NationalId) && x.NationalId.Contains(searchTerm)))
                    .ToList();
            }

            dtos = (SortColumn, SortOrder) switch
            {
                ("Name", "asc") => dtos.OrderBy(x => x.Givenname).ToList(),
                ("Name", "desc") => dtos.OrderByDescending(x => x.Givenname).ToList(),
                ("Adress", "asc") => dtos.OrderBy(x => x.Streetaddress).ToList(),
                ("Adress", "desc") => dtos.OrderByDescending(x => x.Streetaddress).ToList(),
                ("City", "asc") => dtos.OrderBy(x => x.City).ToList(),
                ("City", "desc") => dtos.OrderByDescending(x => x.City).ToList(),
                ("Id", "asc") => dtos.OrderBy(x => x.CustomerId).ToList(),
                ("Id", "desc") => dtos.OrderByDescending(x => x.CustomerId).ToList(),
                ("SSN", "asc") => dtos.OrderBy(x => x.NationalId).ToList(),
                ("SSN", "desc") => dtos.OrderByDescending(x => x.NationalId).ToList(),
                _ => dtos
            };

            TotalPages = (int)Math.Ceiling(dtos.Count / (double)PageSize);
            dtos = dtos
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Customers = _mapper.Map<List<ViewCustomerViewModel>>(dtos);
        }
    }
}
