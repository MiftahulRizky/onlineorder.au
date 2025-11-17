<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Customer_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Customer</h2>
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
                            <h3 class="card-title">Data Customers</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="New Customer" OnClick="btnAdd_Click" />
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="CUSTOMERS DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="30" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="80px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="ExactId" HeaderText="Exact ID" />
                                    <asp:BoundField DataField="Name" HeaderText="Customer Name" />
                                    <asp:BoundField DataField="CustomerGroup" HeaderText="Group" />
                                    <asp:BoundField DataField="CustomerCashSale" HeaderText="Cash Sale" />
                                    <asp:BoundField DataField="CustomerOnStop" HeaderText="On Stop" />
                                    <asp:BoundField DataField="CustomerMinSurcharge" HeaderText="Min. Surcharge" />
                                    <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                    <asp:BoundField DataField="OnStop" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-pill btn-primary" data-bs-toggle="dropdown">Actions</button>
                                            <div class="dropdown-menu dropdown-menu-end">
                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" CommandName="Detail" CommandArgument='<%# Eval("Id") %>' Visible='<%# VisibleDetail(Eval("Id").ToString()) %>'>Detail</asp:LinkButton>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalOnStop" visible='<%# VisibleOnStop(Eval("Id").ToString()) %>' onclick='<%# String.Format("return showOnStop(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), Eval("Name").ToString(), Convert.ToInt32(Eval("OnStop"))) %>'><%# TextOnStop(Eval("OnStop")) %></a>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" visible='<%# VisibleDelete(Eval("Id").ToString()) %>' onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Name").ToString()) %>'>Delete</a>

                                                <div class="dropdown-divider"></div>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" CommandName="Log" CommandArgument='<%# Eval("Id") %>' Text="Logs"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>

                        <div class="card-footer text-start"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAdd" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Customer</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="mb-3 row" runat="server" id="divDebtorCode">
                        <div class="col-4">
                            <label class="form-label required">Id</label>
                            <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Id ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label">Micronet ID</label>
                            <asp:TextBox runat="server" ID="txtMicronetId" CssClass="form-control" placeholder="Micronet ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6" runat="server" id="divExact">
                            <label class="form-label">Exact ID</label>
                            <asp:TextBox runat="server" ID="txtExactId" CssClass="form-control" placeholder="Exact ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-12">
                            <label class="form-label required">Company</label>
                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-12">
                            <label class="form-label required">Customer Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Customer Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label">Customer Group</label>
                            <asp:DropDownList runat="server" ID="ddlGroup" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-6">
                            <label class="form-label">Customer Price Group</label>
                            <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label required">Customer Type</label>
                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Blinds" Text="BLINDS"></asp:ListItem>
                                <asp:ListItem Value="Shutters" Text="SHUTTERS"></asp:ListItem>
                                <asp:ListItem Value="Blinds and Shutters" Text="BLINDS & SHUTTERS"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-6">
                            <label class="form-label required">Sales Person</label>
                            <asp:DropDownList runat="server" ID="ddlSalesPerson" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Office" Text="OFFICE"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-3">
                            <label class="form-label">On Stop</label>
                            <asp:DropDownList runat="server" ID="ddlOnStop" CssClass="form-select">
                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label">Cash Sale</label>
                            <asp:DropDownList runat="server" ID="ddlCashSale" CssClass="form-select">
                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label">Newsletter</label>
                            <asp:DropDownList runat="server" ID="ddlNewsletter" CssClass="form-select">
                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label">Min. Order Surcharge</label>
                            <asp:DropDownList runat="server" ID="ddlMinimumOrderSurcharge" CssClass="form-select">
                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorAdd">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorAdd"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelAdd" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitAdd" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitAdd_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalOnStop" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-info"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-info icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M3 3m0 1a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v4a1 1 0 0 1 -1 1h-4a1 1 0 0 1 -1 -1z" /><path d="M15 15m0 1a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v4a1 1 0 0 1 -1 1h-4a1 1 0 0 1 -1 -1z" /><path d="M21 11v-3a2 2 0 0 0 -2 -2h-6l3 3m0 -6l-3 3" /><path d="M3 13v3a2 2 0 0 0 2 2h6l-3 -3m0 6l3 -3" />
                    </svg>
                    <h3 id="hActive"></h3>
                    <asp:TextBox runat="server" ID="txtIdActive" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActive" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                    <div class="text-secondary">
                        <span id="spanActive"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelOnStop" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitOnStop" CssClass="btn btn-info" Text="Confirm" OnClick="btnSubmitOnStop_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Customer</h3>
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                    <div class="text-secondary">
                        <span id="spanDelete"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="Button1" Text="btnCancelDelete" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnSubmitDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
        
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorLog">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorLog"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA LOG NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# BindTextLog(Eval("Id").ToString()) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        })
        function showAdd() {
            $("#modalAdd").modal("show");
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        function showDelete(id, name) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
            document.getElementById("spanDelete").innerHTML = `<b>${name.toUpperCase()}</b>`;
        }

        function showOnStop(id, name, active) {
            document.getElementById("hActive").innerHTML = active === 1 ? "Change to Online" : "Change to Offline/On Stop";
            document.getElementById("<%=txtIdActive.ClientID %>").value = id;
            document.getElementById("<%=txtActive.ClientID %>").value = active;
            document.getElementById("spanActive").innerHTML = `<br />Customer Name: <br /><b>${name.toUpperCase()}</b>`;
        }
        ["modalAdd", "modalOnStop", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblOnStop"></asp:Label>
        <asp:Label runat="server" ID="lblDesignId"></asp:Label>
    </div>
</asp:Content>

