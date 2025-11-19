<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Mailing.aspx.vb" Inherits="Setting_System_Mailing" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Mailing" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Mailing</h2>
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
                            <h3 class="card-title">Data Mailing</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="MAIL CONFIG NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="AppName" HeaderText="Application Name" />
                                    <asp:BoundField DataField="Name" HeaderText="Name" />
                                    <asp:BoundField DataField="Server" HeaderText="Server" />
                                    <asp:BoundField DataField="Alias" HeaderText="Alias" />
                                    <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="260px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-pill btn-orange" data-bs-toggle="dropdown">Actions</button>
                                            <div class="dropdown-menu dropdown-menu-end">
                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCopy" onclick='<%# String.Format("return showCopy(`{0}`);", Eval("Id").ToString()) %>' visible='<%# VisibleAction() %>'>Copy</a>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>' visible='<%# VisibleAction() %>'>Delete</a>
                                                <div class="dropdown-divider" runat="server"></div>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" Text="Logs" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                        <div class="card-footer text-end"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalProcess" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleProcess"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorProcess"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-5">
                            <label class="form-label required">App Name</label>
                            <asp:DropDownList runat="server" ID="ddlAppId" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-7">
                            <label class="form-label required">Mailing Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Email Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-5">
                            <label class="form-label required">Server</label>
                            <asp:TextBox runat="server" ID="txtServer" CssClass="form-control" placeholder="Server ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-5">
                            <label class="form-label required">Host</label>
                            <asp:TextBox runat="server" ID="txtHost" CssClass="form-control" placeholder="Host ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-2">
                            <label class="form-label required">Port</label>
                            <asp:TextBox runat="server" ID="txtPort" CssClass="form-control" placeholder="Port ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-4">
                            <label class="form-label required">Network Credentials</label>
                            <asp:DropDownList runat="server" ID="ddlNetworkCredentials" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-4">
                            <label class="form-label required">Default Credentials</label>
                            <asp:DropDownList runat="server" ID="ddlDefaultCredentials" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-4">
                            <label class="form-label required">Enable SSL</label>
                            <asp:DropDownList runat="server" ID="ddlEnableSSL" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label">Mail Account</label>
                            <asp:TextBox runat="server" ID="txtAccount" CssClass="form-control" placeholder="Mail Account ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-6">
                            <label class="form-label">Mail Password</label>
                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" placeholder="Mail Password ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label required">Mail Alias</label>
                            <asp:TextBox runat="server" ID="txtAlias" CssClass="form-control" placeholder="Mail Alias ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-6">
                            <label class="form-label required">Mail Subject</label>
                            <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Mail Subject ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label required">Mail To</label>
                            <asp:TextBox runat="server" ID="txtTo" CssClass="form-control" placeholder="Mail To ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                            <small class="form-hint">* Split email with dot comma (;)</small>
                        </div>

                        <div class="col-6">
                            <label class="form-label required">Mail Cc</label>
                            <asp:TextBox runat="server" ID="txtCc" CssClass="form-control" placeholder="Mail CC ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                            <small class="form-hint">* Split email with dot comma (;)</small>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-6">
                            <label class="form-label">Mail Bcc</label>
                            <asp:TextBox runat="server" ID="txtBcc" CssClass="form-control" placeholder="Mail Bcc ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                            <small class="form-hint">* Split email with dot comma (;)</small>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <div class="col-12">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-3">
                            <label class="form-label required">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelProcess" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitProcess" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalCopy" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-secondary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-secondary icon-lg">
                      <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path stroke="none" d="M0 0h24v24H0z" /><path d="M7 9.667a2.667 2.667 0 0 1 2.667 -2.667h8.666a2.667 2.667 0 0 1 2.667 2.667v8.666a2.667 2.667 0 0 1 -2.667 2.667h-8.666a2.667 2.667 0 0 1 -2.667 -2.667z" /><path d="M4.012 16.737a2 2 0 0 1 -1.012 -1.737v-10c0 -1.1 .9 -2 2 -2h10c.75 0 1.158 .385 1.5 1" /><path d="M11 14h6" /><path d="M14 11v6" />
                    </svg>
                    <h3>Copy Email Config</h3>
                    <asp:TextBox runat="server" ID="txtIdCopy" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCanceCopy" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitCopy" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnSubmitCopy_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Email Config</h3>
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelDelete" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
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

        function showProcess() {
            $("#modalProcess").modal("show");
        }
        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
        function showCopy(id) {
            document.getElementById("<%=txtIdCopy.ClientID %>").value = id;
        }        
        function showLog() {
            $("#modalLog").modal("show");
        }
        ["modalProcess", "modalCopy", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>