<%@ Page Title="Product Type" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_ProductType_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">


    <script type="text/javascript">
        let URIMETHOD = "/Methods/Setting/ProductType/ProductTypeMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Setting/ProductType/ProductType.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

