namespace Desk.Document
{
    // Auto lets the content decide:
    // item title/date/author -> Header, page images -> Media, sections -> Main.
    public enum PageRegion
    {
        Auto,
        Header,
        Media,
        Main,
        Side
    }
}