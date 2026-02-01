using Application.Dtos.DistrictDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CityDtos;

public class CityWithDistrictResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<DistrictItemResponse> Districts { get; set; }
}
