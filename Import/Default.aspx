<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Import_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Import Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Import Order</h2>
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
                            <h3 class="card-title">IMPORT ORDER FROM SOE</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-6 row">
                                <div class="col-lg-12">
                                    <div class="form-label">Upload File</div>
                                    <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control " />
                                    <small class="form-hint">* CSV format only !</small>
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
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" >
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i>Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        const inputFile = document.getElementById('MainContent_fuFile');
        inputFile.addEventListener('change', function (e) {
            // e.target.classList.remove('is-invalid');
        })

        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
            });
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblStoreId"></asp:Label>
        <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
        <asp:Label runat="server" ID="lblOrderCust"></asp:Label>
        <asp:Label runat="server" ID="lblStatus"></asp:Label>
        <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
        <asp:Label runat="server" ID="lblSubmittedDate"></asp:Label>
        <asp:Label runat="server" ID="lblShipmentDate"></asp:Label>
        <asp:Label runat="server" ID="lblShipmentNo"></asp:Label>
        <asp:Label runat="server" ID="lblContainerNo"></asp:Label>
        <asp:Label runat="server" ID="lblCourier"></asp:Label>
        
        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderHeaders VALUES (@Id, @UserId, @StoreId, @OrderNo, @OrderCust,  NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, @Status, @CreatedDate, NULL, @SubmittedDate, NULL, NULL, NULL, NULL, @ShipmentDate, @ShipmentNo, @ContainerNo, @Courier, NULL, 1)" UpdateCommand="UPDATE OrderHeaders SET Status=@Status, ShipmentDate=@ShipmentDate, ShipmentNo=@ShipmentNo, ContainerNo=@ContainerNo, Courier=@Courier WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblStoreId" Name="StoreId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblOrderNo" Name="OrderNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblOrderCust" Name="OrderCust" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblStatus" Name="Status" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCreatedDate" Name="CreatedDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSubmittedDate" Name="SubmittedDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblShipmentDate" Name="ShipmentDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblShipmentNo" Name="ShipmentNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblContainerNo" Name="ContainerNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCourier" Name="Courier" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblStatus" Name="Status" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblShipmentDate" Name="ShipmentDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblShipmentNo" Name="ShipmentNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblContainerNo" Name="ContainerNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCourier" Name="Courier" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>