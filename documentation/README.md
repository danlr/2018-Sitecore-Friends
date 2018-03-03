# Expirience Profile Editor. Documentation.
![Hackathon Logo](images/hackathon.png?raw=true "Hackathon Logo")
## Summary

**Category:** xConnect

Sitecore Experience Profile provides single custom view in a readonly mode. Experience profile data is populated from xDB and gives marketers valuable insights in contact behaviour and profile. Our module is intended to make contact experience profile editable. We created a Speak UI component that makes it possible to update standard contact facets right from the Experience Profile app using xConnect client API. Data is instantly saved to Sitecore xDB. We made it possible to edit default contact facets.

It is easy to customize Sitecore contact facets to save additional information. Custom contact facets are deployed to Sitecore & xConnect. However custom facets are only accessible from code and can only be seen in Collection database. Marketers don't have any way to see custom facets associated with contact. Our module solves this problem. We created additional tab in Experience Profile where all custom facets are rendered using xConnect client API.

Browsing experience profile to see contact custom facets will also be extremely helpful for developers to review if facets are deployed and updated correct.

## Pre-requisites

- xConnect up and running

## Installation

 - Install the package ![Package](/sc.package/XEditor-1.0.zip?raw=true "package")
 - Deploy demo facet to xConnect ![Files](/sc.package/xConnect "files")

## Usage

Once module is installed marketers can immediately see new components in Experience Profile app.

![Edit contact details form](images/EditContactForm.png?raw=true "Edit contact details form")
![Edit contact details form](images/EditContactFormAfterEdit.png?raw=true "Edit contact details form")
![Edit contact all facets form](images/EditAllFacetsForm.png?raw=true "Edit contact all facets form")

You will need some contacts in your xConnect to see the module in action. You can generate a couple of test contacts via special link http://[sitecore_url]/api/sitecore/ContactApi/AddTestData

## Video


[![Sitecore Hackathon Video](sitecore-friends-2018.mp4 )](sitecore-friends-2018.mp4?raw=true)
