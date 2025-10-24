<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Order_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="List Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">List Order</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Order</h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnAdd" CssClass="btn btn-primary rounded-pill"  OnClick="btnAdd_Click" >
                                    <i class="fa-solid fa-plus" style="margin-right: 0.3rem;"></i>
                                    Create Order
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3">
                            <div class="d-flex">
                                <div class="text-secondary">
                                    <div class="ms-2 d-inline-block">
                                        <asp:DropDownList runat="server" ID="ddlStatus" Width="200px" CssClass="form-select rounded-pill" ToolTip="Status Order" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="ms-auto  text-center">
                                    <div class="ms-2 d-inline-block">
                                        Please note that all draft orders will be removed from the system if there are no activities after 10 days. <br />
                                        <b>Your order will go into order cancellation.</b><br />
                                        Thank you for your attention.
                                    </div>
                                </div>
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control rounded-pill" placeholder="Search Data" autocomplete="off" ToolTip="Search Data Order ....."></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary rounded-pill"  OnClick="btnSearch_Click" >
                                                <i class="fa-solid fa-magnifying-glass" style="margin-right: 0.3rem;"></i>
                                                Search
                                            </asp:LinkButton>
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
                            <asp:GridView runat="server" ID="gvList" Width="100%" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" PagerSettings-Position="TopAndBottom" EmptyDataText="ORDER DATA NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50" ToolTip="List Order" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="80px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                    <asp:TemplateField HeaderText="JOB NO">
                                        <ItemTemplate>
                                            <%# TrigerJoNumber(Eval("JoNumber").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="StoreName" HeaderText="STORE NAME" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="OrderNo" HeaderText="ORDER NUMBER" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="OrderCust" HeaderText="REFERENCE" ItemStyle-Wrap="true" />
                                    <%-- <asp:BoundField DataField="Delivery" HeaderText="DELIVERY" /> --%>
                                    <asp:TemplateField HeaderText="DELIVERY">
                                        <ItemTemplate>
                                            <%# TrigerDelivery(Eval("Delivery").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STATUS">
                                        <ItemTemplate>
                                            <span class="status-order">
                                                <%# Eval("Status").ToString() %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ORDER DATE">
                                        <ItemTemplate>
                                            <div class="text-center">
                                                <button type="button" class="btn btn-sm btn-outline-warning btn-pill" title="Information Order Date" data-bs-toggle="modal" data-bs-target="#modalOrderDate" onclick='<%# String.Format("return showInfo(`{0}`, `{1}`, `{2}`, `{3}`, `{4}`, `{5}`);", Eval("OrderNo"), Eval("OrderCust"), Eval("CreatedDate", "{0: MMMM, dd yyyy}"), Eval("SubmittedDate", "{0: MMMM, dd yyyy}"), Eval("CompletedDate", "{0: MMMM, dd yyyy}"), Eval("CanceledDate", "{0: MMMM, dd yyyy}")) %>'>
                                                    <i class="bi bi-calendar-date-fill opacity-80 me-2"></i> Information date
                                                </button>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ACTIONS">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                    <li>
                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkDetail" OnClick="linkDetail_Click">
                                                            <i class="bi bi-info-circle me-2 opacity-50"></i> Detail
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item text-danger " data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}` , `{2}`);", Eval("Id").ToString(), Eval("OrderNo").ToString(), Eval("OrderCust")) %>' visible='<%# VisibleDelete(Eval("Status").ToString(), Eval("UserId").ToString())%>'>
                                                            <i class="bi bi-trash3 me-2"></i>Delete
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkChange" OnClick="linkChange_Click" Visible='<%# VisibleChange(Eval("Status").ToString(), Eval("Active").ToString()) %>'>
                                                            <i class="bi bi-clipboard-check me-2 opacity-50"></i>Change Status
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <button type="button" class="dropdown-item btnDowloadCsv" id="btnDowloadCsv" data-id='<%# Eval("Id").ToString() %>' data-status='Eval("Status").ToString()'>
                                                            <i class="bi bi-file-earmark-arrow-down me-2 opacity-50"></i> Download CSV Order 
                                                        </button>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalRestore" onclick='<%# String.Format("return showRestore(`{0}`, `{1}` , `{2}`);", Eval("Id").ToString(), Eval("OrderNo").ToString(), Eval("OrderCust")) %>' visible='<%# VisibleRestore(Eval("Active")) %>'>
                                                            <i class="bi bi-arrow-repeat me-2 opacity-50"></i>Restore
                                                        </a>
                                                    </li>
                                                </ul>
                                              </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <%-- <AlternatingRowStyle BackColor="White" /> --%>
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>

                            <asp:Table runat="server" ID="tblInfo" CssClass="table table-transparent table-responsive mt-2">
                                <asp:TableRow>
                                    <asp:TableCell Width="100%" HorizontalAlign="Center">
                                        Please note that all draft orders will be removed from the system if there are no activities after 10 days. <br />
                                        <b>Your order will go into order cancellation.</b><br />
                                        Thank you for your attention.
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </div>

                        <div class="card-footer">
                            <div class="ms-2 d-inline-block" runat="server" id="divActive">
                                <asp:DropDownList runat="server" ID="ddlActive" Width="200px" CssClass="form-select rounded-pill" AutoPostBack="true" OnSelectedIndexChanged="ddlActive_SelectedIndexChanged">
                                    <asp:ListItem Value="1" Text="ACTIVE DATA"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="NON ACTIVE"></asp:ListItem>
                                </asp:DropDownList>
                                <small class="form-hint">* Sort Data</small>
                            </div>
                            
                            <div class="ms-2 d-inline-block" runat="server" id="divStoreType">
                                <asp:DropDownList runat="server" ID="ddlStoreType" Width="150px" CssClass="form-select rounded-pill" AutoPostBack="true" OnSelectedIndexChanged="ddlStoreType_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="ALL"></asp:ListItem>
                                    <asp:ListItem Value="REGULAR" Text="REGULAR"></asp:ListItem>
                                    <asp:ListItem Value="PRO FORMA" Text="PRO FORMA"></asp:ListItem>
                                </asp:DropDownList>
                                <small class="form-hint">* Store Type</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalOrderDate" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="hPo"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-4">
                        <label class="form-label">1. CREATED DATE :</label>
                        <asp:TextBox runat="server" ID="txtCreatedDate" CssClass="form-control rounded-pill" placeholder="Created Date ...." ReadOnly="true"></asp:TextBox>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">2. SUBMITTED DATE :</label>
                        <asp:TextBox runat="server" ID="txtSubmittedDate" CssClass="form-control rounded-pill" placeholder="Submitted Date ...." ReadOnly="true"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">3. COMPLETED DATE :</label>
                        <asp:TextBox runat="server" ID="txtCompletedDate" CssClass="form-control rounded-pill" placeholder="Completed Date ...." ReadOnly="true"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">4. CANCELED DATE :</label>
                        <asp:TextBox runat="server" ID="txtCanceledDate" CssClass="form-control rounded-pill" placeholder="Canceled Date ...." ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-warning rounded-pill" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Order</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                        <span id="spanDescription"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger w-100" Text="Yes. Delete" OnClick="btnDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalRestore" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-refresh">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M20 11a8.1 8.1 0 0 0 -15.5 -2m-.5 -4v4h4" />
                        <path d="M4 13a8.1 8.1 0 0 0 15.5 2m.5 4v-4h-4" />
                    </svg>
                    <h3>Restore Order</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdRestore" style="display:none;"></asp:TextBox>
                        <span id="spanDescriptionRestore"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnRestore" CssClass="btn btn-danger w-100" Text="Yes. Do it" OnClick="btnRestore_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showDelete(id, orderNumber, orderName) {
            var description = 'Are you sure you want to delete order? <br /><br />';
            description += '- ORDER NUMBER : ' + orderNumber;
            description += '<br />';
            description += '- REFERENCE : ' + orderName;
            description += '<br /><br />';
            description += 'What you have done cannot be undone.';
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
            document.getElementById("spanDescription").innerHTML = description;
        }
        function showRestore(id, orderNumber, orderName) {
            var description = 'Are you sure you want to restore this order? <br /><br />';
            description += '- ORDER NUMBER : ' + orderNumber;
            description += '<br />';
            description += '- REFERENCE : ' + orderName;
            document.getElementById('<%=txtIdRestore.ClientID %>').value = id;
            document.getElementById("spanDescriptionRestore").innerHTML = description;
        }
        function showInfo(OrderNo, OrderCust, Created, Submitted, Completed, Canceled) {
            document.getElementById("hPo").innerHTML = '#' + OrderNo + ' ' + OrderCust;

            document.getElementById('<%=txtCreatedDate.ClientID %>').value = Created;           
            document.getElementById('<%=txtSubmittedDate.ClientID %>').value = Submitted;
            document.getElementById('<%=txtCompletedDate.ClientID %>').value = Completed;
            document.getElementById('<%=txtCanceledDate.ClientID %>').value = Canceled;
        }
        let roleName = '<%= Session("RoleName") %>';

    </script>
    <script src="../Scripts/OrderHeaderPage/OrderHeaderPage.js" type="text/javascript"></script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblUserId"></asp:Label>

        <asp:SqlDataSource ID="sdsDelete" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Active=0 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsRestore" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='Draft', Active=1 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>