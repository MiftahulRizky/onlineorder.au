<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Welcome.aspx.vb" Inherits="System_Welcome" MasterPageFile="~/Error.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Welcome" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <div class="text-center">
         <div class="py-7">
             <a runat="server" href="~/" class="navbar-brand navbar-brand-autodark"><img runat="server" src="~/Content/static/LS.jpeg" height="100" width="300" alt=""></a>
         </div>
         <div class="text-secondary mb-3">
             <h2>The system is currently not activated</h2>
         </div>
     </div>
</asp:Content>
