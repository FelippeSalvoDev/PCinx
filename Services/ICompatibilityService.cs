using pcinx_api.Models;

namespace pcinx_api.Services;

public interface ICompatibilityService
{
    List<Message> Validate(List<Part> selectedParts);
}

