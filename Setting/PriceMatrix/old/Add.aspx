<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_PriceMatrix_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Matrix" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Price Matrix</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-7">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Add Price</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">DESIGN TYPE</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">PRICE GROUP</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Pick Up" Text="PICK UP"></asp:ListItem>
                                        <asp:ListItem Value="Delivery" Text="FIS / DELIVERY"></asp:ListItem>
                                        <asp:ListItem Value="INT-FIS" Text="INT-FIS"></asp:ListItem>
                                        <asp:ListItem Value="INT-PU" Text="INT-PU"></asp:ListItem>                                        
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">WIDTH x DROP</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtDrop" TextMode="Number" CssClass="form-control" placeholder="Drop ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">COST</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCost" CssClass="form-control" placeholder="Cost ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
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

                        <div class="card-footer text-end">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblPriceGroup"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO Prices VALUES (NEWID(), @PriceGroup, @Type, @Drop, @Width, @Cost)">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblPriceGroup" Name="PriceGroup" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlType" Name="Type" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCost" Name="Cost" PropertyName="Text" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
