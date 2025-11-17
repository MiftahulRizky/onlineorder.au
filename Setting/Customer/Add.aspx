<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Customer_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Customer" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">
                        <a runat="server" href="~/setting/customer" class="text-decoration-none">Customer</a>
                    </h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Add Customer</h3>
                        </div>

                        <div class="card-body">
                            <div class="datagrid mb-5">
                                <div class="datagrid-item" runat="server" id="divId">
                                    <div class="datagrid-title datagridTitle">CUSTOMER ID</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Web ID ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ACCOUNT</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlAccount" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Master" Text="MASTER CUSTOMER"></asp:ListItem>
                                            <asp:ListItem Value="Sub" Text="SUB CUSTOMER"></asp:ListItem>
                                            <asp:ListItem Value="Regular" Text="REGULAR CUSTOMER"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item" runat="server" id="divMaster">
                                    <div class="datagrid-title datagridTitle">MASTER ID</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlMaster" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5" runat="server" id="divInternal">
                                <div class="datagrid-item" runat="server" id="divExact">
                                    <div class="datagrid-title datagridTitle">EXACT ID</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:TextBox runat="server" ID="txtExactId" CssClass="form-control" placeholder="Exact ID ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">CUSTOMER NAME</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Customer Name ...." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">CUSTOMER PRICE GROUP</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ON STOP</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlOnStop" CssClass="form-select">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">CASH SALE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlCashSale" CssClass="form-select">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">NEWSLETTER</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlNewsletter" CssClass="form-select">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">MIN. ORDER SURCHARGE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlMinimumOrderSurcharge" CssClass="form-select">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                     <div class="datagrid-title datagridTitle">ACTIVE</div>
                                     <div class="datagrid-content datagridContent">
                                         <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" Width="25%">
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                            <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                        </asp:DropDownList>
                                     </div>
                                 </div>
                            </div>

                            <div class="row" runat="server" id="divError">
                                <div class="col-lg-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                        <div class="d-flex">
                                            <div>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                            </div>
                                            <div>
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-start">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
