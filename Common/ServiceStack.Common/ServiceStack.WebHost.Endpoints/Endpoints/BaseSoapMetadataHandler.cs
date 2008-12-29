using System;
using System.Web;
using System.Web.UI;
using System.Xml.Schema;
using ServiceStack.WebHost.Endpoints.Support.Endpoints;
using ServiceStack.WebHost.Endpoints.Support.Endpoints.Controls;
using ServiceStack.WebHost.Endpoints.Utils;

namespace ServiceStack.WebHost.Endpoints.Endpoints
{
    public abstract class BaseSoapMetadataHandler : BaseMetadataHandler
    {
    	public override void ProcessRequest(HttpContext context)
    	{
			Request = context.Request;
			var operations = new ServiceOperations(ServiceOperationType, OperationVerbs.ReplyOperationVerbs, OperationVerbs.OneWayOperationVerbs);

    		if (context.Request.QueryString["xsd"] != null)
    		{
    			var xsdNo = Convert.ToInt32(context.Request.QueryString["xsd"]);
    			var schemaSet = XsdUtils.GetXmlSchemaSet(operations.AllOperations.Types);
    			var schemas = schemaSet.Schemas();
    			var i = 0;
    			if (xsdNo >= schemas.Count)
    			{
    				throw new ArgumentOutOfRangeException("xsd");
    			}
    			context.Response.ContentType = "text/xml";
    			foreach (XmlSchema schema in schemaSet.Schemas())
    			{
    				if (xsdNo != i++) continue;
    				schema.Write(context.Response.Output);
    				break;
    			}
    			return;
    		}

    		var writer = new HtmlTextWriter(context.Response.Output);
    		context.Response.ContentType = "text/html";
    		ProcessOperations(writer);
    	}

    	protected override void RenderOperations(HtmlTextWriter writer, Operations allOperations)
    	{
			var defaultPage = new IndexOperationsControl {
				Title = this.ServiceName,
				Xsds = XsdTypes.Xsds,
				OperationNames = allOperations.Names,
				UsageExamplesBaseUri = this.UsageExamplesBaseUri,
			};

			defaultPage.RenderControl(writer);
		}

    }
}