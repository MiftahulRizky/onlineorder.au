<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Maintenance.aspx.vb" Inherits="Error_Maintenance" MasterPageFile="~/Error.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Maintenance" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="empty-img">
        <asp:Image runat="server" Height="128px" ImageUrl="/Content/static/undraw_quitting_time_dm8t.svg" />
    </div>
    <p class="empty-title">Temporarily down for maintenance</p>
    <p class="empty-subtitle text-secondary">
        Sorry for the inconvenience but we’re performing some maintenance at the moment. We’ll be back online shortly!
    </p>
    
    <div class="empty-action">
        <a runat="server" href="~/" class="btn btn-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l14 0" /><path d="M5 12l6 6" /><path d="M5 12l6 -6" />
            </svg>
            Take me home
        </a>
    </div>
</asp:Content>