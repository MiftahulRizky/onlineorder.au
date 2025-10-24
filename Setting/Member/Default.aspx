<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Setting_Member_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Membership"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Memberships</h2>
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
                            <h3 class="card-title">Data Memberships</h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnAdd" CssClass="btn btn-primary " Text="Add New" OnClick="btnAdd_Click" >
                                    <i class="fa-solid fa-plus" style="margin-right: 0.3rem;"></i> Add New
                                </asp:LinkButton>

                                <button class="btn btn-secondary dropdown-toggle " data-bs-toggle="dropdown">
                                    <i class="fa-solid fa-ellipsis-vertical" style="margin-right: 0.3rem;"></i> Actions
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                    <asp:LinkButton runat="server" ID="linkOnline" CssClass="dropdown-item" Text="Online Member" OnClick="linkOnline_Click"></asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="linkActivityAll" CssClass="dropdown-item" Text="Activity Member" OnClick="linkActivityAll_Click"></asp:LinkButton>
                                </div>
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3">
                            <div class="d-flex">
                                <div class="text-secondary"></div>
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control " placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary " Text="Search" OnClick="btnSearch_Click" >
                                                <i class="fa-solid fa-magnifying-glass" style="margin-right: 0.3rem;"></i> Search
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" PagerSettings-Position="TopAndBottom" EmptyDataText="MEMBER DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserId" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField ItemStyle-Width="180px" HeaderText="USERNAME">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkDetail" OnClick="linkDetail_Click" visible='<%# VisibleUsername(Eval("UserName").ToString()) %>'>
                                                <%# IconActiveOnNames(Eval("LockoutEnabled").ToString(), Eval("UserName").ToString()) %>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="StoreId" HeaderText="STORE" ItemStyle-Width="180px" />
                                    <asp:BoundField DataField="Access" HeaderText="ACCESS" ItemStyle-Width="250px" />
                                    <asp:BoundField DataField="LastActivityDate" HeaderText="LAST LOGIN" HeaderStyle-Width="200px" DataFormatString="{0:MMM dd, yyyy HH:mm:ss}" />
                                    <asp:BoundField DataField="LockoutEnabled" HeaderText="LOCKOUT ENABLED" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField ItemStyle-Width="270px">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <%-- <a href="#" title="Delete" runat="server" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("UserId").ToString(), Eval("UserName").ToString()) %>' visible='<%# VisibleDelete(Eval("UserName").ToString()) %>' style="height: 2.3em; width: 2em;">
                                                    <i class="bi bi-trash3"></i>
                                                </a>
                                                
                                                <asp:LinkButton runat="server" title="Switch Activated" CssClass='<%# CssActive(Eval("LockoutEnabled")) %>' ID="linkActivityMember" Text='<%# TextActive(Eval("LockoutEnabled").ToString()) %>' Visible='<%# VisibleActive(Eval("UserName").ToString()) %>' OnClick="linkActive_Click" style="height: 2.3em; width: 2em;"></asp:LinkButton>
                                                
                                                <asp:LinkButton runat="server" title="Activity" CssClass="btn  btn-warning" ID="linkActivity" Text="Activity" OnClick="linkActivity_Click" Visible='<%# VisibleActivity() %>' style="height: 2.3em; width: 2em;">
                                                    <i class="fa-solid fa-person"></i>
                                                </asp:LinkButton>
                                                
                                                <button runat="server" title="Password" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" style="height: 2.3em; width: 2em;" visible='<%# VisibleResetPass(Eval("UserName").ToString()) %>'>
                                                    <i class="bi bi-key fs-3"></i>
                                                </button>
                                                <div class="dropdown-menu dropdown-menu-end">
                                                    <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalReset" onclick='<%# String.Format("return showReset(`{0}`, `{1}`);", Eval("UserId").ToString(), Eval("UserName").ToString()) %>' hidden> Reset Pass </a>
                                                    <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPassword" onclick='<%# String.Format("return showPassword(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>' > Show Pass </a>
                                                </div> --%>



                                                <button class="border-0 bg-transparent  dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item text-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("UserId").ToString(), Eval("UserName").ToString()) %>' visible='<%# VisibleDelete(Eval("UserName").ToString()) %>' style="height: 2.3em; width: 2em;">
                                                            <i class="bi bi-trash3 me-2"></i>Delete
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" CssClass='<%# CssActive(Eval("LockoutEnabled")) %>' ID="linkActivityMember" Text='<%# TextActive(Eval("LockoutEnabled").ToString()) %>' Visible='<%# VisibleActive(Eval("UserName").ToString()) %>' OnClick="linkActive_Click" style="height: 2.3em; width: 2em;"></asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkActivity" OnClick="linkActivity_Click" Visible='<%# VisibleActivity() %>'>
                                                            <i class="bi bi-person-vcard me-2 opacity-50"></i>Activity
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <a runat="server" class="dropdown-item" href="javascript:void(0)" visible='<%# VisibleResetPass(Eval("UserName").ToString()) %>'>
                                                            <i class="bi bi-key me-2 opacity-50"></i>Password
                                                        </a>
                                                        <ul class="dropdown-menu dropdown-submenu-left dropdown-submenu">
                                                            <li>
                                                                <a href="javascript:void(0)" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalReset" onclick='<%# String.Format("return showReset(`{0}`, `{1}`);", Eval("UserId").ToString(), Eval("UserName").ToString()) %>'> 
                                                                    Reset Pass
                                                                </a>
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0)" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPassword" onclick='<%# String.Format("return showPassword(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>' >
                                                                    Show Pass
                                                                </a>
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                    </div>
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
                    <h3>Delete Data?</h3>
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

    <div class="modal modal-blur fade" id="modalReset" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-primary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-primary icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Reset Password</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdReset" style="display:none;"></asp:TextBox>
                        <span id="spanDescriptionReset"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnReset" CssClass="btn btn-primary w-100" Text="Yes. Reset It" OnClick="btnReset_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalPassword" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status"></div>
                <div class="modal-body text-center py-4">
                    <h3>Show Password</h3>
                    <div class="text-secondary">
                        <span id="spanPassword"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn btn-primary w-100" data-bs-dismiss="modal">Close</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
        function showDelete(id, name) {
            var result = 'Are you sure you want to delete this account?';
            result += '<br /><br />';
            result += '<b>DATA : ' + name.toUpperCase() + ' | ' + id.toUpperCase() + '</b>';
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
            document.getElementById("spanDescription").innerHTML = result;
        }

        function showPassword(username, password) {
            var result = 'USERNAME : ' + username;
            result += '<br />';
            result += 'PASSWORD : ' + password;
            document.getElementById("spanPassword").innerHTML = result;
        }

        function showReset(id, name) {
            var result = 'The password will change to : <b>123456</b>';
            result += '<br />';
            result += 'Are you sure you want to reset this account password?';
            result += '<br />';
            result += '<b>' + name.toUpperCase() + ' | ' + id.toUpperCase() + '</b>';
            document.getElementById('<%=txtIdReset.ClientID %>').value = id;
            document.getElementById("spanDescriptionReset").innerHTML = result;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblLockoutEnabled"></asp:Label>
        <asp:Label runat="server" ID="lblPassword"></asp:Label>
        <asp:Label runat="server" ID="lblPasswordHash"></asp:Label>

        <asp:SqlDataSource ID="sdsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" DeleteCommand="DELETE FROM Users WHERE UserId=@UserId">
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsMemberhips" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET LockoutEnabled=@LockoutEnabled WHERE UserId=@UserId" DeleteCommand="DELETE FROM Memberships WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblLockoutEnabled" Name="LockoutEnabled" PropertyName="Text" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsResetPassword" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET Password=@PasswordHash WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPassword" Name="Password" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="PasswordHash" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsActive" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET Password=@Password, PasswordHash=@PasswordHash WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPassword" Name="Password" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="PasswordHash" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>