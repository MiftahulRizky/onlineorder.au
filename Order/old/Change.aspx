<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Change.aspx.vb" Inherits="Order_Change" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Change Status" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">ORDER</div>
                    <h2 class="page-title">Change Status</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3">
                <div class="col-lg-6 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Table runat="server" Font-Size="Larger" CellPadding="5">
                                        <asp:TableRow>
                                            <asp:TableCell Width="130px">Order Number</asp:TableCell>
                                            <asp:TableCell Width="15px">:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>Reference</asp:TableCell>
                                            <asp:TableCell>:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderCust"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Change Status Form</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">STATUS</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                    <small runat="server" id="statusOld" class="form-hint"></small>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divSubmittedDate">
                                <label class="col-lg-3 col-form-label required">SUBMITTED DATE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtSubmittedDate" TextMode="Date" CssClass="form-control" placeholder="Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divCompletedDate">
                                <label class="col-lg-3 col-form-label">DATE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCompletedDate" TextMode="Date" CssClass="form-control" placeholder="Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divCanceledDate">
                                <label class="col-lg-3 col-form-label required">CANCELED DATE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCanceledDate" TextMode="Date" CssClass="form-control" placeholder="Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 mt-6 row" runat="server" id="divDescription">
                                <label class="col-lg-3 col-form-label required">DESCRIPTION</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" CssClass="form-control" Height="100px" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
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
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Information</h3>
                        </div>

                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        // Function untuk menampilkan pesan error dari code-behind
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
            });
        }
    </script>
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblStatusOri"></asp:Label>
        <asp:Label runat="server" ID="lblDescription"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsDraft" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='Draft', StatusDescription=NULL, SubmittedDate=NULL, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsNewOrder" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='New Order', StatusDescription=@StatusDescription, SubmittedDate=@SubmittedDate, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDescription" Name="StatusDescription" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtSubmittedDate" Name="SubmittedDate" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsProduction" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='In Production', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDescription" Name="StatusDescription" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtSubmittedDate" Name="SubmittedDate" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsHold" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='On Hold', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDescription" Name="StatusDescription" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtSubmittedDate" Name="SubmittedDate" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsCanceled" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=@CanceledDate WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCanceledDate" Name="CanceledDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDescription" Name="StatusDescription" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsCompleted" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='Completed', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=@CompletedDate WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCompletedDate" Name="CompletedDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDescription" Name="StatusDescription" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
