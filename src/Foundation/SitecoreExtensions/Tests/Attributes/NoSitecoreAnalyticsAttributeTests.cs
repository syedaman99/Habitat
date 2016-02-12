﻿namespace Sitecore.Foundation.SitecoreExtensions.Tests.Attributes
{
  using System;
  using System.Collections.Specialized;
  using System.Web.Mvc;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Analytics;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Foundation.SitecoreExtensions.Attributes;
  using Sitecore.Mvc.Common;
  using Sitecore.Mvc.Presentation;
  using Xunit;

  public class NoSitecoreAnalyticsAttributeTests
  {
    [Theory]
    [AutoDbMvcData]
    public void OnActionExecuting_OnAjaxRequest_ShouldCallCancelAnalytics(NoSitecoreAnalyticsAttribute attribute, ActionExecutingContext ctx, ITracker tracker)
    {
      //arrange
      InitializeActionFilterContext(ctx);
      tracker.IsActive.Returns(true);

      using (new TrackerSwitcher(tracker))
      {
        //act
        attribute.OnActionExecuting(ctx);
        //assert
        tracker.CurrentPage.Received(1).Cancel();
      }
     
    }


    [Theory]
    [AutoDbMvcData]
    public void OnActionExecuting_TrackerNotInitialized_ShouldDoNothing(NoSitecoreAnalyticsAttribute attribute, ActionExecutingContext ctx, ITracker tracker)
    {
      //arrange
      InitializeActionFilterContext(ctx);

      using (new TrackerSwitcher(tracker))
      {
        //act
        attribute.OnActionExecuting(ctx);
        //assert
        tracker.CurrentPage.DidNotReceive().Cancel();
      }
    }


    [Theory]
    [AutoDbMvcData]
    public void OnActionExecuting_CurrentPageIsNull_ShouldNotRaiseException(NoSitecoreAnalyticsAttribute attribute, ActionExecutingContext ctx, ITracker tracker)
    {
      //arrange
      InitializeActionFilterContext(ctx);
      tracker.IsActive.Returns(true);
      tracker.CurrentPage.Returns((ICurrentPageContext)null);


      using (new TrackerSwitcher(tracker))
      {
        //act
        Action action = () => attribute.OnActionExecuting(ctx);
        //assert
        action.ShouldNotThrow();
      }
    }


    private static void InitializeActionFilterContext(ActionExecutingContext ctx)
    {
      ctx.RequestContext.HttpContext.Request.Headers.Returns(new NameValueCollection());
      ctx.RequestContext.HttpContext.Request.Headers.Add("X-Requested-With", "XMLHttpRequest");

    }
  }
}