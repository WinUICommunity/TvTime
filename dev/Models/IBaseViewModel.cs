using System.Windows.Input;

namespace TvTime.Models;
public interface IBaseViewModel
{
    public ICommand MenuFlyoutItemCommand { get; }
}
