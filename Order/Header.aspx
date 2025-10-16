<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Header.aspx.vb" Inherits="Order_Header" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Create Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Order</div>
                    <h2 runat="server">Order Header</h2>
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
                            <h3 class="card-title" runat="server" id="cardTitle"></h3>
                        </div>
                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">STORE NAME</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStore" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row" runat="server" id="divFindStore">
                                <label class="col-lg-3 col-form-label">FIND BY STORE ID</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStore" CssClass="form-control " placeholder="Find store id ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                     <asp:LinkButton runat="server" ID="btnFindStore" CssClass="btn btn-primary " Text="Find" OnClick="btnFindStore_Click" >
                                        <i class="bi bi-search me-2"></i> Find
                                     </asp:LinkButton>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ORDER NUMBER</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtOrderNo" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                                    <small class="form-hint" style="color:red;">* Please quote this order number when querying this order</small>
                                </div>
                            </div>

                            <div class="mb-4 row">
                                <label class="col-lg-3 col-form-label required">REFERENCE</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtReference" CssClass="form-control" placeholder="Reference ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-lg-3 col-form-label required">DELIVERY / PICK UP</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDelivery" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Delivery" Text="DELIVERY"></asp:ListItem>
                                        <asp:ListItem Value="Pick Up" Text="PICK UP"></asp:ListItem>
                                        <asp:ListItem Value="INT-FIS" Text="INT-FIS"></asp:ListItem>
                                        <asp:ListItem Value="INT-PU" Text="INT-PU"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">NOTE</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtNote" Height="120px" TextMode="MultiLine" CssClass="form-control" placeholder="Your note for this order ..." autocomplete="off" style="resize:none;"></asp:TextBox>
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

                        <div class="card-footer text-end">
                             <asp:LinkButton runat="server" ID="btnSubmit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" >
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit 
                             </asp:LinkButton>
                             <asp:LinkButton runat="server" ID="btnCancel" CssClass="btn btn-danger " OnClick="btnCancel_Click">
                                <i class="fa-solid fa-arrow-rotate-left me-2"></i> Cancel
                             </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="markdown">
                                <ul>
                                    <li>
                                        Please do not use the following characters for <br />
                                        <b>ORDER NUMBER</b> & <b>REFERENCE</b> <br />
                                        <b>[ / ], [ | ], [ \ ], [ & ], [ # ], [ ' ], [ ` ] AND [ , ]</b>
                                    </li>
                                    <li class="mt-2">
                                        MAXIMUM 50 CHARACTERS FOR <b>ORDER NUMBER</b>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- custom js -->
    <script type="text/javascript" src="/Content/dist/js/my/header.js"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
        })
        // Function untuk menampilkan pesan error dari code-behind
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
                },
            });
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
       
        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderHeaders(Id, UserId, StoreId, OrderNo, OrderCust, Delivery, Note, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, CreatedDate, Active) VALUES (@Id, @UserId, @StoreId, LTRIM(RTRIM(@OrderNo)), LTRIM(RTRIM(@OrderCust)), @Delivery, @Note, 'Yes', 0, 0, 0,  'Draft', GETDATE(), 1)" UpdateCommand="UPDATE OrderHeaders SET UserId=@UserId, StoreId=@StoreId, OrderNo=LTRIM(RTRIM(@OrderNo)), OrderCust=LTRIM(RTRIM(@OrderCust)), Delivery=@Delivery, Note=@Note, Active=1 WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStore" Name="StoreId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtOrderNo" Name="OrderNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtReference" Name="OrderCust" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDelivery" Name="Delivery" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNote" Name="Note" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStore" Name="StoreId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtOrderNo" Name="OrderNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtReference" Name="OrderCust" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDelivery" Name="Delivery" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNote" Name="Note" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>