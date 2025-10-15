<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Query.aspx.vb" Inherits="Setting_Query" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Query" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Run Query</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3" runat="server" id="divError">
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

            <div class="row mb-5">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Run Manual Query</h3>
                        </div>
                        <div class="card-body">
                            <div class="mb-5 row">
                                <label class="col-lg-2 col-form-label">ACTION QUERY</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlAction" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Create" Text="CREATE"></asp:ListItem>
                                        <asp:ListItem Value="Read" Text="READ"></asp:ListItem>
                                        <asp:ListItem Value="Update" Text="UPDATE"></asp:ListItem>
                                        <asp:ListItem Value="Delete" Text="DELETE"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <label class="form-label">YOUR QUERY</label>
                                    <asp:TextBox runat="server" TextMode="MultiLine" Height="150px" ID="txtQuery" CssClass="form-control" placeholder="Your Query ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 col-md-12 col-sm-12">                    
                    <div class="row">
                        <div class="card">
                            <div class="card-header">
                                <h3 class="card-title">Notes</h3>
                            </div>
                            <div class="card-body">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3" runat="server" id="divErrors">
                <div class="col-lg-12">
                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                        <div class="d-flex">
                            <div>
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                            </div>
                            <div>
                                <span runat="server" id="msgErrors"></span>
                            </div>
                        </div>
                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                    </div>
                </div>
            </div>

            <div class="row" runat="server" id="divList">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">List</h3>
                        </div>
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="true" EmptyDataText="Data not found ... :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"></asp:SqlDataSource>
    </div>
</asp:Content>