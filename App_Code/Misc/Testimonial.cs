using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for Testimonials
/// </summary>
public class Testimonial : BaseTableObject
{
    #region COLUMNS

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "Testimonials"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Body")]
    public string Body { get { return _Body; } set { _Body = value; SetUpToDateAsFalse(); } }

    [Column("Signature")]
    public string Signature { get { return _Signature; } set { _Signature = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    public TestimonialStatus Status
    {
        get { return (TestimonialStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _id, _UserId, _Status;
    string _Body, _Signature;
    #endregion

    public Testimonial()
            : base() { }

    public Testimonial(int id) : base(id) { }

    public Testimonial(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    public static void Add(int userId, string body, string signature)
    {
        Testimonial testimonial = new Testimonial();
        testimonial.UserId = userId;
        testimonial.Body = body;
        testimonial.Signature = signature;
        testimonial.Status = TestimonialStatus.Paused;
        testimonial.Save();
    }

    public static List<Testimonial> Get(TestimonialStatus status)
    {
        return TableHelper.GetListFromRawQuery<Testimonial>(string.Format("SELECT * FROM Testimonials WHERE Status = {0}", (int)status));
    }

    public static string GetTestimonials()
    {
        TestimonialsCache cache = new TestimonialsCache();

        var testimonials = (IEnumerable<Testimonial>)cache.Get();

        StringBuilder sb = new StringBuilder();

        sb.Append("<div class=\"carousel testimonials slide\" data-ride=\"carousel\" id=\"testimonials\"><div class=\"carousel-inner text-center\">");

        for (var i = 0; i < testimonials.Count(); i++)
        {
            sb.Append("<div class=\"item" + (testimonials.ElementAt(i) == testimonials.ElementAt(0) ? " active" : "") + "\"><blockquote><i class=\"fa fa-quote-left\"></i>");
            sb.Append(testimonials.ElementAt(i).Body);
            sb.Append("<i class=\"fa fa-quote-right\"></i></blockquote>");
            sb.Append("<div class=\"name\"> - <span class=\"text-theme\">");
            sb.Append(testimonials.ElementAt(i).Signature);
            sb.Append("</span></div>");
            sb.Append("</div>");
        }

        sb.Append("</div><ol class=\"carousel-indicators\">");

        for (var i = 0; i < testimonials.Count(); i++)
        {                                                
            sb.Append("<li data-target=\"#testimonials\" data-slide-to=\"" + i + "\" class=\"" + (testimonials.ElementAt(i) == testimonials.ElementAt(0) ? "active" : "") + "\"></li>");
        }

        sb.Append("</ol></div>");

        return sb.ToString();
    }
}