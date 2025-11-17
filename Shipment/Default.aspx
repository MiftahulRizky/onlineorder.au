<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Shipment_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Shipment" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Shipment</div>
                    <h2 class="page-title">List Shipment</h2>
                </div>
            </div>
        </div>
    </div>
    
    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Shipment</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="New Shipment" OnClick="btnAdd_Click" />
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3">
                            <div class="d-flex">
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3" runat="server" id="divError">
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

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" Font-Size="15px" Width="100%" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" PagerSettings-Position="Top" EmptyDataText="SHIPMENT DATA NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="ShipmentNumber" HeaderText="Shipment Number" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="ETAPort" HeaderText="ETA to Port" DataFormatString="{0:MMM dd, yyyy}" />
                                    <asp:BoundField DataField="ETACustomer" HeaderText="ETA to Customer" DataFormatString="{0:MMM dd, yyyy}" />
                                    <asp:BoundField DataField="FullName" HeaderText="Created By" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:MMM dd, yyyy}" />
                                    <asp:TemplateField ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="linkDetail" CssClass="btn btn-sm btn-pill btn-primary" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>

                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>