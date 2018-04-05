using System;
using Prem.PTC.Members;
using SocialNetwork;
using System.Collections.Generic;
using System.Web.UI;
using System.Reflection;
using Prem.PTC;
using System.Web.UI.WebControls;

public partial class PostControl : System.Web.UI.UserControl
{
    public Post Post { get; set; }
    public Member User { get; set; }
    public Member Author { get; set; }
    private List<Comment> comments { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.CurrentInCache;
        Author = new Member(Post.AuthorId);
        AddLang();
        LoadPost();

        if (!IsPostBack)
        {
            CommentsPlaceHolder.Visible = false;
            NewCommentPlaceHolder.Visible = false;
        }

        if (CancelCommentButton.Visible)
        {
            UpdateCommentsList();
            LoadComments();
        }
        else
        {
            AddCommentButton.Visible = true;
            CommentsPlaceHolder.Visible = false;
            NewCommentPlaceHolder.Visible = false;
        }
    }

    private void UpdateCommentsList()
    {
        comments = Post.Comments;
    }
    private void AddLang()
    {
        AddCommentButton.Text = "<i class=\"fa fa-comments fa-fw\"></i> Comment";
        ConfirmCommentButton.Text = "Add";
        CancelCommentButton.Text = "<i class=\"fa fa-times fa-fw\"></i> Cancel";
    }

    private void LoadPost()
    {
        PostImage.ImageUrl = Post.ImagePath;
        PostText.InnerText = Post.Text;
        PostDate.InnerText = Post.DateTime.ToShortDateString();
        PostTime.InnerText = Post.DateTime.ToShortTimeString();

        PostAuthor.InnerHtml = string.Format("<a href=\"{0}\">{1}</a>", HtmlCreator.GetProfileURL(Author.Id, Author.Name), Author.Name);
        PostAuthorImage.ImageUrl = Author.AvatarUrl;
    }

    private void LoadComments()
    {
        CommentsPlaceHolder.Visible = true;
        CommentsDiv.Controls.Clear();

        for (int i = 0; i < comments.Count; i++)
        {
            UserControl commentControl = (UserControl)Page.LoadControl("~/Controls/Network/Comment.ascx");

            PropertyInfo comment = commentControl.GetType().GetProperty("Comment");
            comment.SetValue(commentControl, comments[i], null);

            PropertyInfo post = commentControl.GetType().GetProperty("Post");
            post.SetValue(commentControl, Post, null);

            ((ICommentObjectControl)commentControl).DeleteButtonClicked += CommentControl_DeleteButtonClicked;

            commentControl.DataBind();
            CommentsDiv.Controls.Add(commentControl);
        }
    }

    private void CommentControl_DeleteButtonClicked(object sender, EventArgs e)
    {
        UpdateCommentsList();
        LoadComments();
    }

    #region Add Comment
    Banner tempImage;

    public Banner TempImage
    {
        get
        {
            if (tempImage == null)
            {
                if (ViewState["TempImage"] == null)
                {
                    ViewState["TempImage"] = Banner.Empty;
                }

                tempImage = ViewState["TempImage"] as Banner;
            }
            return tempImage;
        }
        set
        {

            tempImage = value;
            ViewState["TempImage"] = tempImage;
        }
    }

    protected void ImageUploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;

            DeleteOldImage();
            if (!Banner.TryFromStream(ImageUpload.PostedFile.InputStream, out tempImage))
                throw new MsgException("Select an image first");

            Comment.ValidateImage(tempImage.Width, tempImage.Height);
            TempImage = TempImage.Save(AppSettings.FolderPaths.PostImages);

            ImagePreview.ImageUrl = tempImage.Path;

            ClearUpload();
        }
        catch (Exception ex)
        {
            EPanel.Visible = true;
            ELabel.Text = ex.Message;
            DeleteOldImage();
        }
    }

    private void ClearUpload()
    {
        ImageUpload.Dispose();
    }

    private void DeleteOldImage()
    {
        if (TempImage != null && TempImage.IsSaved)
            TempImage.Delete();

    }

    private void ClearAll()
    {
        TempImage = null;
        ImageUpload.Visible = true;
        ImageUploadButton.Visible = true;
        ImagePreview.ImageUrl = string.Empty;
        NewCommentTextBox.Text = string.Empty;
    }

    protected void AddCommentButton_Click(object sender, EventArgs e)
    {
        UpdateCommentsList();
        LoadComments();
        NewCommentPlaceHolder.Visible = true;
        AddCommentButton.Visible = false;
        CancelCommentButton.Visible = true;
    }
    protected void ConfirmCommentButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;

            var text = InputChecker.HtmlEncode(NewCommentTextBox.Text, NewCommentTextBox.MaxLength, "Post");
            Post.AddComment(Member.CurrentId, text, TempImage);

            ClearAll();
            UpdateCommentsList();
            LoadComments();
            NewCommentPlaceHolder.Visible = true;
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                ELabel.Text = ex.Message;
                DeleteOldImage();
            }
        }
    }
    #endregion

    protected void CancelCommentButton_Click(object sender, EventArgs e)
    {
        ClearAll();
        NewCommentPlaceHolder.Visible = CommentsPlaceHolder.Visible = false;
        AddCommentButton.Visible = true;
    }
}
