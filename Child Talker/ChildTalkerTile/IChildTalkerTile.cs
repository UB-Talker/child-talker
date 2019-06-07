namespace Child_Talker
{
    public interface IChildTalkerTile
    {
        bool IsLink();
        void PerformAction();
        string Text { get; set; }
        string ImagePath { get; set; }
        IChildTalkerTile Parent { get; set; }
        ChildTalkerXml Xml { get; set; }
    }
}

