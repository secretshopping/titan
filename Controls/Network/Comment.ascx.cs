using System;
using Prem.PTC.Members;
using SocialNetwork;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Reflection;

public partial class CommentControl : System.Web.UI.UserControl, ICommentObjectControl
{
    public Post Post { get; set; }
    public Comment Comment { get; set; }
    public Member User { get; set; }

    public event EventHandler DeleteButtonClicked;

    public override void DataBind()
    {
        base.DataBind();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        User = new Member(Comment.AuthorId);

        LoadComment();
        AddLang();

    }

    private void AddLang()
    {
        DeleteCommentButton.Text = "Delete";
    }

    private void LoadComment()
    {
        CommentImage.ImageUrl = Comment.ImagePath;
        CommentText.InnerText = Comment.Text;

        CommentTime.InnerText = Comment.DateTime.ToShortTimeString();
        CommentAuthor.InnerHtml = string.Format("<a class=\"text-primary\" href=\"{0}\">{1}</a>", HtmlCreator.GetProfileURL(User.Id, User.Name), User.Name);

        DeleteCommentButton.Visible = Post.AuthorId == Member.CurrentId || Comment.AuthorId == Member.CurrentId;
    }

    protected void DeleteCommentButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;
            Comment.Delete(Member.CurrentId);

            //Works like this: http://stackoverflow.com/questions/623136/calling-a-method-in-parent-page-from-user-control
            if (DeleteButtonClicked != null)
                DeleteButtonClicked(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            EPanel.Visible = true;
            ELabel.Text = ex.Message;
        }
    }
}
