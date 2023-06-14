namespace TvTime.Common;
public class ListViewItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextBlockTemplate { get; set; }
    public DataTemplate HyperLinkTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        switch (Settings.DescriptionType)
        {
            case DescriptionType.TextBlock:
                return TextBlockTemplate;
            case DescriptionType.HyperLink:
                return HyperLinkTemplate;
            default:
                return base.SelectTemplateCore(item);
        }
    }
}
