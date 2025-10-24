<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Tutorial_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Tutorial" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Tutorial</h2>
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
                            <h3 class="card-title">List Tutorial</h3>
                            <div class="card-actions">
                                <%-- <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary " Text="Add New" OnClick="btnAdd_Click" /> --%>
                                 <asp:LinkButton runat="server" ID="btnAdd" CssClass="btn btn-primary " OnClick="btnAdd_Click">
                                    <i class="fa-regular fa-plus me-2"></i> Add New
                                 </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body py-3">
                            <div class="d-flex">
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control " placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary " OnClick="btnSearch_Click">
                                                <i class="fa-solid fa-magnifying-glass me-2"></i> Search
                                            </asp:LinkButton>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="card-body py-3" runat="server" id="divError">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                            <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                                            <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0"></path>
                                            <path d="M12 8v4"></path>
                                            <path d="M12 16h.01"></path>
                                        </svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgError"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>

                        <div class="card-body py-3">
                            <div class="row row-cards">
                                <asp:ListView runat="server" ID="lvData" OnItemCommand="lvData_ItemCommand">
                                    <ItemTemplate>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="card">
                                                <asp:Image runat="server" ImageUrl=<%# String.Format("~/File/Tutorial/Thumbnail/{0}", Eval("Thumbnail").ToString()) %> />
                                                <div class="card-body">
                                                    <h3 class="card-title"><%# Eval("Title").ToString() %></h3>
                                                    <p class="text-secondary">
                                                        <%# Eval("NewDesc").ToString() %>
                                                    </p>
                                                </div>

                                                <div class="card-footer text-end">
                                                    <asp:LinkButton runat="server" CssClass="btn btn-primary " Text="Download PDF" CommandName="Unduh" CommandArgument='<%# Eval("File").ToString() %>'></asp:LinkButton>

                                                    <asp:LinkButton runat="server" CssClass="btn btn-info " Text="Read More ..." CommandName="Lihat" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>

                                                    <button runat="server" class="btn dropdown-toggle " data-bs-toggle="dropdown" visible='<%# VisibleAction() %>'>Actions</button>
                                                    <div class="dropdown-menu dropdown-menu-start">
                                                        <asp:LinkButton runat="server" ID="linkEdit" CssClass="dropdown-item" Text="Edit" CommandName="Ubah" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>
                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Title").ToString()) %>'> Delete </a>
                                                        
                                                        <asp:LinkButton runat="server" ID="linkActive" CssClass="dropdown-item" Text='<%# TextActive(Eval("Active").ToString()) %>' CommandName="Aktif" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
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

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        })

        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                }
            });
        }
        function showDelete(id, name) {
            var result = 'Are you sure you want to delete this tutorial?';
            result += '<br /><br />';
            result += '<b>DATA : ' + name.toUpperCase() + '</b>';
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
            document.getElementById("spanDescription").innerHTML = result;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Tutorials SET Active=0 WHERE Id=@Id" DeleteCommand="DELETE FROM Tutorials WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </UpdateParameters>

            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>