using Titan.EBooks;

public interface IEBookObjectControl
{
    EBook Object { get; set; }
    void DataBind();
}