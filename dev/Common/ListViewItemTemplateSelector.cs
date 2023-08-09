namespace TvTime.Common;
public class ListViewItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextBlockTemplate { get; set; }
    public DataTemplate HyperLinkTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        switch (Settings.DescriptionTemplate)
        {
            case DescriptionTemplateType.TextBlock:
                return TextBlockTemplate;
            case DescriptionTemplateType.HyperLink:
                return HyperLinkTemplate;
            default:
                return base.SelectTemplateCore(item);
        }
    }
}
