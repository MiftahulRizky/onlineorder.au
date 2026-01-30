<%@ Page Language="VB" AutoEventWireup="false" CodeFile="408.aspx.vb" Inherits="System_408" MasterPageFile="~/Error.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="408" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="empty-header">408</div>
    <p class="empty-title">Oops… You just found an error page</p>
    <p class="empty-subtitle text-secondary">
        We are sorry but the page you are looking for was not found
    </p>
    <div class="empty-action">
        <a runat="server" href="~/" class="btn btn-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l14 0" /><path d="M5 12l6 6" /><path d="M5 12l6 -6" /></svg>
            Take me home
        </a>
    </div>
</asp:Content>
