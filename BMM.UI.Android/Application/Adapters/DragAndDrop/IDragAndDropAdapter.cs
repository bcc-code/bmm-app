namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public interface IDragAndDropAdapter
    {
        bool OnItemMove(int fromPosition, int toPosition);
    }
}