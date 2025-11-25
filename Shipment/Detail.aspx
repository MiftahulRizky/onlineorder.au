<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Shipment_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Shipment" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-4">
                    <div class="page-pretitle">Shipment</div>
                    <h2 class="page-title">Detail Shipment</h2>
                </div>

                <div class="col-8 text-end">
                    <a href="#" runat="server" id="aComplete" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalComplete">Complete Order</a>
                    <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-secondary" Text="Edit" OnClick="btnEdit_Click" />
                    <a href="#" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
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

            <div class="row mb-3">
                <div class="col-7">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Shipment Number</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblShipmentNumber"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ETA to Port</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblEtaPort"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ETA to Customer</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblEtaCustomer"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-5">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Created Date</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Created By</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h2 class="card-title">ORDERS</h2>
                        </div>

                        <div class="card-body py-3" runat="server" id="divEmail">
                            <div class="d-flex">
                                <div class=" text-secondary">
                                    <div class="ms-2 d-inline-block">
                                        <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalNewShipment">Email New Shipment</a>
                                    </div>
                                    <div class="ms-2 d-inline-block">
                                        <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalAmendedShipment">Email Amended ETA</a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="ORDER ITEM NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkRow" runat="server" AutoPostBack="true" OnCheckedChanged="chkRow_CheckedChanged" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                                    <asp:BoundField DataField="OrderName" HeaderText="Order Name" />
                                    <asp:BoundField DataField="Term" HeaderText="Term" />
                                    <asp:BoundField DataField="CustOnStop" HeaderText="On Stop" />
                                    <asp:TemplateField HeaderText="Primary Contact">
                                        <ItemTemplate>
                                            <%# BindPrimaryContact(Eval("CustomerId")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalEdit" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Shipment</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">Shipment Number</label>
                            <asp:TextBox runat="server" ID="txtShipmentNumber" CssClass="form-control" placeholder="Shipment Number" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">ETA to Port</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtETAPort" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">ETA to Customer</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtETACutomer" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <div runat="server" id="divErrorEdit" class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorEdit"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitEdit" CssClass="btn btn-primary w-100" Text="Submit" OnClick="btnSubmitEdit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalComplete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-primary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-primary icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Complete Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                        <br /><br />
                        <b>This action cannot be reversed.</b>
                        <br />
                        It will update the status of each orders to Completed.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitComplete" CssClass="btn w-100 btn-primary" Text="Confirm" OnClick="btnSubmitComplete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Shipment</h3>
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

    <div class="modal modal-blur fade" id="modalNewShipment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-success icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M3 7a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v10a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-10z" />
                        <path d="M3 7l9 6l9 -6" />
                    </svg>
                    <h3>Email New Shipment</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitNewShipment" CssClass="btn btn-success w-100" Text="Confirm" OnClick="btnSubmitNewShipment_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAmendedShipment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-success icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M3 7a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v10a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-10z" />
                        <path d="M3 7l9 6l9 -6" />
                    </svg>
                    <h3>Email Amended ETA</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitAmendedShipment" CssClass="btn btn-success w-100" Text="Confirm" OnClick="btnSubmitAmendedShipment_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblShipmentId"></asp:Label>
        <asp:Label runat="server" ID="lblCompleted"></asp:Label>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
        function showEdit() {
            $('#modalEdit').modal('show');
        }

        ["modalEdit", "modalComplete", "modalDelete", "modalNewShipment", "modalAmendedShipment"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        document.addEventListener("DOMContentLoaded", function () {
            var etaPort = document.getElementById('<%= txtETAPort.ClientID %>');
            var etaCustomer = document.getElementById('<%= txtETACutomer.ClientID %>');

            if (etaPort && etaCustomer) {
                etaPort.addEventListener('change', function () {
                    var etaPortDate = new Date(etaPort.value);
                    if (!isNaN(etaPortDate.getTime())) {
                        etaPortDate.setDate(etaPortDate.getDate() + 14);
                        var year = etaPortDate.getFullYear();
                        var month = String(etaPortDate.getMonth() + 1).padStart(2, '0');
                        var day = String(etaPortDate.getDate()).padStart(2, '0');
                        etaCustomer.value = `${year}-${month}-${day}`;
                    }
                });
            }
        });
    </script>
</asp:Content>
