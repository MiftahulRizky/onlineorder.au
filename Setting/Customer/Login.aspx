<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Setting_Customer_Login" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Customer Login</h2>
                </div>
            </div>
        </div>
    </div>
    <div class="page-body">
        <div class="container-xl">
            <div class="row" runat="server" id="divError">
                <div class="col-12">
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

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Customer Login</h3>
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

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="CUSTOMER LOGIN DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="30" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Id" HeaderText="ID" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="AppName" HeaderText="Application" />
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="FullName" HeaderText="FullName" />
                                    <asp:BoundField DataField="UserName" HeaderText="User" />
                                    <asp:BoundField DataField="RoleName" HeaderText="Role" />
                                    <asp:BoundField DataField="LevelName" HeaderText="Level" />
                                    <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                    <asp:TemplateField HeaderText="Active">
                                        <ItemTemplate>
                                            <div runat="server" visible='<%# VisiblePrimaryYes(Eval("Active")) %>'>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-circle-check">
                                                    <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M9 12l2 2l4 -4" />
                                                </svg>
                                            </div>
                                            <div runat="server" visible='<%# VisiblePrimaryNo(Eval("Active")) %>'>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-circle-x"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M10 10l4 4m0 -4l-4 4" />
                                                </svg>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-pill btn-purple" data-bs-toggle="dropdown">Actions</button>
                                            <div class="dropdown-menu dropdown-menu-end">
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkEdit" OnClick="linkEdit_Click">Edit</asp:LinkButton>

                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalActive" onclick='<%# String.Format("return showActive(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'><%# TextActive_Login(Eval("Active").ToString()) %></a>

                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>

                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalResetPass" onclick='<%# String.Format("return showResetPass(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("UserName").ToString()) %>'>Reset Password</a>

                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDencryptPass" onclick='<%# String.Format("return showDencryptPass(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>'>Show Password</a>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </div>
                        <div class="card-footer text-end"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalProccess" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleLogin"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-4 row" runat="server" id="divApplication">
                        <div class="col-12">
                            <label class="form-label required">Application</label>
                            <asp:DropDownList runat="server" ID="ddlApplication" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-4 row">
                        <div class="col-12">
                            <label class="form-label required">Customer</label>
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-4 row" runat="server" id="divAccess">
                        <div class="col-6">
                            <label class="form-label required">Role</label>
                            <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-6">
                            <label class="form-label required">Level</label>
                            <asp:DropDownList runat="server" ID="ddlLevel" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-4 row">
                        <div class="col-12">
                            <label class="form-label required">Full Name</label>
                            <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-4 row">
                        <div class="col-6">
                            <label class="form-label required">Username</label>
                            <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control" placeholder="UserName ..." autocomplete="new-password"></asp:TextBox>
                        </div>
                        <div class="col-6" runat="server" id="divPassword">
                            <label class="form-label">Password</label>
                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" placeholder="Password ..."></asp:TextBox>
                            <small class="form-hint" id="passwordinfo"></small>
                        </div>
                    </div>
                
                    <div class="row" runat="server" id="divErrorProccess">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorProccess"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            
                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProccessLogin" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitProccess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalActive" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3 id="hActive"></h3>
                    <asp:TextBox runat="server" ID="txtIdActive" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActive" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActive" CssClass="btn btn-danger" Text="Confirm" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalResetPass" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-primary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-primary icon-lg"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M3.06 13a9 9 0 1 0 .49 -4.087" /><path d="M3 4.001v5h5" /><path d="M12 12m-1 0a1 1 0 1 0 2 0a1 1 0 1 0 -2 0" /></svg>
                    <asp:TextBox runat="server" ID="txtIdResetPass" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtNewResetPass" style="display:none;"></asp:TextBox>
                    <h3>Reset Password</h3>
                    <div class="text-secondary">                        
                        <span id="spanDescResetPass"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetPass" CssClass="btn btn-primary" Text="Confirm" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDencryptPass" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-primary icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M16.555 3.843l3.602 3.602a2.877 2.877 0 0 1 0 4.069l-2.643 2.643a2.877 2.877 0 0 1 -4.069 0l-.301 -.301l-6.558 6.558a2 2 0 0 1 -1.239 .578l-.175 .008h-1.172a1 1 0 0 1 -.993 -.883l-.007 -.117v-1.172a2 2 0 0 1 .467 -1.284l.119 -.13l.414 -.414h2v-2h2v-2l2.144 -2.144l-.301 -.301a2.877 2.877 0 0 1 0 -4.069l2.643 -2.643a2.877 2.877 0 0 1 4.069 0z" /><path d="M15 9h.01" />
                    </svg>
                    <h3>Show Password</h3>
                    <div class="text-secondary">
                        <span id="spanPassword"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-primary" data-bs-dismiss="modal">Close</a>
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
                    <h3>Delete Customer Price Group</h3>
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        });
        function showProccess() {
            $("#modalProccess").modal("show");
        }
        function showActive(id, active) {
            document.getElementById("<%=txtIdActive.ClientID %>").value = id;
            document.getElementById("<%=txtActive.ClientID %>").value = active;

            let title = "";
            if (active === 1) {
                title = "Disable Customer Login";
            } else {
                title = "Enable Customer Login";
            }
            document.getElementById("hActive").innerHTML = title;
        }
        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
        function showResetPass(id, username) {
            let newPass = generateNewPassword(15);
            let result = `New password : <b>${newPass}</b><br />Are you sure you want to reset this account password?<br /><br /><b>USERNAME : ${username.toUpperCase()}</b><br /><b>USER ID : ${id.toUpperCase()}</b>`;

            document.getElementById("<%=txtIdResetPass.ClientID %>").value = id;
            document.getElementById("<%=txtNewResetPass.ClientID %>").value = newPass;
            document.getElementById("spanDescResetPass").innerHTML = result;
        }
        function showDencryptPass(username, password) {
            let body = "UserName";
            body += "<br />";
            body += "<b>" + username + "</b>";
            body += "<br /><br />";
            body += "Password Decryption";
            body += "<br />";
            body += "<b><u>" + password + "</u></b>";
            document.getElementById("spanPassword").innerHTML = body;
        }

        // Handle password info display
        var txtUserName = document.getElementById("<%= txtUserName.ClientID %>");
        var passwordInfo = document.getElementById("passwordinfo");

        if (txtUserName) {
            txtUserName.addEventListener("input", function () {
                passwordInfo.innerHTML = txtUserName.value.trim() === "" ? "" : `* If the password input is empty, the password will be <b>${txtUserName.value}</b>`;
            });
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
        <asp:Label runat="server" ID="lblLoginUserNameOld"></asp:Label>

        <asp:Label runat="server" ID="lblAppId"></asp:Label>
        <asp:Label runat="server" ID="lblRoleId"></asp:Label>
        <asp:Label runat="server" ID="lblLevelId"></asp:Label>
        <asp:Label runat="server" ID="lblPasswordHash"></asp:Label>
        <asp:Label runat="server" ID="lblAdditional"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO CustomerLogins VALUES (NEWID(), GETDATE(), GETDATE(), @AppId, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, NULL, 0, NULL, 1, 1)" UpdateCommand="UPDATE CustomerLogins SET UpdatedDate=GETDATE(), ApplicationId=@AppId, CustomerId=@CustomerId, RoleId=@RoleId, LevelId=@LevelId, UserName=@UserName, FullName=@FullName WHERE Id=@Id" DeleteCommand="DELETE FROM CustomerLogins WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="ddlCustomer" Name="CustomerId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblAppId" Name="AppId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblRoleId" Name="RoleId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblLevelId" Name="LevelId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtUserName" Name="UserName" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="Password" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFullName" Name="FullName" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlCustomer" Name="CustomerId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblAppId" Name="AppId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblRoleId" Name="RoleId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblLevelId" Name="LevelId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtUserName" Name="UserName" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFullName" Name="FullName" PropertyName="Text" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsActive" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE CustomerLogins SET Active=@Active, UpdatedDate=GETDATE() WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblAdditional" Name="Active" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsResetPass" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE CustomerLogins SET Password=@Password, Reset=1 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="Password" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>