<%@ Page Title="Create Orders" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CreateOrders.aspx.vb" Inherits="Order_CreateOrders"  MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Order</div>
                    <h2 runat="server">Create Order</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-7">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="titleCard">Create New Order</h3>
                        </div>
                        <div class="card-body">
                            <div class="row mb-3" id="divOrderType">
                                <label for="ordertype" class="col-lg-4 form-label text-uppercase required">
                                    order type
                                </label>
                                <div class="col-lg-4">
                                    <select name="ordertype" id="ordertype" class="form-select "></select>
                                    <input type="text" name="headerid" id="headerid" class="form-control" readonly hidden>
                                </div>
                            </div>
                            <div id="formDetail">
                                <div class="row mb-5" id="divCustomer">
                                    <label for="customer" class="col-lg-4 form-label text-uppercase required" id="lblcustomer">customer name</label>
                                    <div class="col-lg-8">
                                        <select name="customer" id="customer" class="form-select "></select>
                                        <input type="text" name="customerold" id="customerold" class="form-control" readonly hidden >
                                    </div>
                                </div>

                                <div class="row mb-3" id="divCreatedBy">
                                    <label for="createdby" class="col-lg-4 form-label text-uppercase required">created by</label>
                                    <div class="col-lg-8">
                                        <select name="createdby" id="createdby" class="form-select"></select>
                                    </div>
                                </div>
                                <div class="row mb-5" id="divCreatedDate">
                                    <label for="createddate" class="col-lg-4 form-label text-uppercase required">created date</label>
                                    <div class="col-lg-3">
                                        <input type="date" class="form-control" id="createddate" name="createddate">
                                    </div>
                                </div>

                                <div class="row mb-3" id="divOrderId">
                                    <label for="orderid" class="col-lg-4 form-label text-uppercase required">order id</label>
                                    <div class="col-lg-4">
                                        <input type="text" class="form-control " id="orderid" name="orderid" placeholder="Order Number ...">
                                    </div>
                                </div>

                                <div class="row mb-3" id="divCreateType">
                                    <label for="createtype" class="col-lg-4 form-label text-uppercase required">create type</label>
                                    <div class="col-lg-4">
                                        <select name="createtype" id="createtype" class="form-select ">
                                            <option value="Create New">CREATE NEW</option>
                                            <option value="Upload CSV">UPLOAD CSV</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divFiles">
                                    <label for="files" class="col-lg-4 form-label text-uppercase required">CSV FILE</label>
                                    <div class="col-lg-8">
                                        <input type="file" class="form-control" id="files" name="files" accept=".csv" />
                                    </div>
                                </div>

                                <div class="row mb-3" id="divOrderNumber">
                                    <label for="ordernumber" class="col-lg-4 form-label text-uppercase required">order number</label>
                                    <div class="col-lg-6">
                                        <div class="input-group">
                                            <input type="text" class="form-control " id="ordernumber" name="ordernumber" placeholder="Order Number ...">
                                            <button class="btn btn-information" type="button" id="btnInfoOrderNumber">?</button>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb-5" id="divOrderName">
                                    <label for="ordername" class="col-lg-4 form-label text-uppercase required" id="lblOrderName">order name</label>
                                    <div class="col-lg-8">
                                        <div class="input-group">
                                            <input type="text" class="form-control " id="ordername" name="ordername" placeholder="Order Name ...">
                                            <button class="btn btn-information" type="button" id="btnInfoOrderName">?</button>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divDelivery">
                                    <label for="delivery" class="col-lg-4 form-label text-uppercase required">delivery / pick up</label>
                                    <div class="col-lg-4">
                                        <div class="input-group">
                                            <select name="delivery" id="delivery" class="form-select ">
                                                <option value=""></option>
                                                <option value="Delivery">DELIVERY</option>
                                                <option value="Pick Up">PICK UP</option>
                                                <option value="INT-FIS">INT-FIS</option>
                                                <option value="INT-PU">INT-PU</option>
                                            </select>
                                            <button class="btn btn-information" type="button" id="btnInfoDelivery">?</button>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divDeliveryType">
                                    <label for="deliverytype" class="col-lg-4 form-label text-uppercase">delivery type</label>
                                    <div class="col-lg-4">
                                        <div class="input-group">
                                            <select name="deliverytype" id="deliverytype" class="form-select ">
                                                <option value=""></option>
                                                <option value="By Van">BY VAN</option>
                                                <option value="Third Party">THIRD PARTY</option>
                                            </select>
                                            <button class="btn btn-information" type="button" id="btnInfoDeliveryType">?</button>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divSource">
                                    <label for="sourcetype" class="col-lg-4 form-label text-uppercase required">source type</label>
                                    <div class="col-lg-4">
                                        <input type="text" class="form-control " id="sourcetypeold" name="sourcetypeold" readonly hidden>
                                        <div class="input-group">
                                            <select name="sourcetype" id="sourcetype" class="form-select ">
                                                <option value="New">NEW</option>
                                                <option value="Remake">REMAKE</option>
                                            </select>
                                            <button class="btn btn-information" type="button" id="btnInfoSourceType">?</button>
                                        </div>
                                    </div>
                                </div>


                                <div class="row mb-3">
                                    <label for="notes" class="col-lg-4 form-label text-uppercase">notes</label>
                                    <div class="col-lg-8">
                                        <textarea name="notes" id="notes" class="form-control" rows="4" cols="4" placeholder="Your note for this order ..."></textarea>
                                        <span class="form-label-description" id="notescount">0/1000</span>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divJobId">
                                    <label for="jobid" class="col-lg-4 form-label text-uppercase required">job id</label>
                                    <div class="col-lg-4">
                                      <input type="text" class="form-control" id="jobid" name="jobid">
                                    </div>
                                </div>

                                <div class="row mb-3" id="divJobDate">
                                    <label for="jobdate" class="col-lg-4 form-label text-uppercase required">job date</label>
                                    <div class="col-lg-4">
                                      <input type="date" class="form-control" id="jobdate" name="jobdate">
                                    </div>
                                </div>

                                <div class="row mb-3" id="divShipmentId">
                                    <label for="shipmentid" class="col-lg-4 form-label text-uppercase">shipment number</label>
                                    <div class="col-lg-4">
                                      <select name="shipmentid" id="shipmentid" class="form-select"></select>
                                    </div>
                                </div>

                                <div class="row mb-3" id="divShipping">
                                    <label for="shippingaddress" class="col-lg-4 form-label text-uppercase">shipping address</label>
                                    <div class="col-lg-8">
                                        <textarea name="shippingaddress" id="shippingaddress" class="form-control" rows="3" cols="3" ></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <button type="button" class="btn btn-primary" id="btn-submit-order">Next</button>
                            <button type="button" class="btn btn-danger" id="btn-cancel">Cancel</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <!-- Modal -->
    <div class="modal fade" id="modalShipping" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalShippingLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalShippingLabel">Modal title</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <form action="javascript:void(0);" method="post" id="form-submit">     
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-lg-4 mb-3">
                                <label for="unitnumber" class="form-label">Unit Number</label>
                                <input type="text" name="unitnumber" id="unitnumber" class="form-control" placeholder="Unit Number ..." autocomplete="off">
                                <input type="text" name="customer" id="customer" class="form-control" readonly hidden>
                                <input type="text" name="id" id="id" class="form-control" readonly hidden>
                            </div>
                            <div class="col-lg-8 mb-3">
                                <label for="streetaddress" class="form-label required">Street Address</label>
                                <textarea name="streetaddress" id="streetaddress" class="form-control" cols="1" rows="1" placeholder="Street Address ..." autocomplete="off"></textarea>
                            </div>
                            <div class="col-lg-6 mb-3">
                                <label for="suburb" class="form-label required">Suburb</label>
                                <input type="text" name="suburb" id="suburb" class="form-control"  placeholder="Suburb ..." autocomplete="off">
                            </div>
                            <div class="col-lg-6 mb-3">
                                <label for="states" class="form-label required">States</label>
                                <select name="states" id="states" class="form-select">
                                    <option value=""></option>
                                    <option value="ACT">ACT</option>
                                    <option value="NSW">NSW</option>
                                    <option value="NT">NT</option>
                                    <option value="QLD">QLD</option>
                                    <option value="SA">SA</option>
                                    <option value="TAS">TAS</option>
                                    <option value="VIC">VIC</option>
                                    <option value="WA">WA</option>
                                </select>
                            </div>
                            <div class="col-lg-6 mb-3">
                                <label for="postcode" class="form-label required">Post Code</label>
                                <textarea name="postcode" id="postcode" class="form-control" cols="1" rows="1"  placeholder="Post Code ..." autocomplete="off"></textarea>
                            </div>
                            <div class="col-lg-6 mb-3">
                                <label for="addressport" class="form-label required">Nearest Port</label>
                                <select name="addressport" id="addressport" class="form-select">
                                    <option value=""></option>
                                    <option value="Adelaide">Adelaide</option>
                                    <option value="Brisbane">Brisbane</option>
                                    <option value="Melbourne">Melbourne</option>
                                    <option value="Perth">Perth</option>
                                    <option value="Sydney">Sydney</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" id="btn-submit-shipping">Save Changes</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        const params = new URLSearchParams(window.location.search);
        const ACTION = params.get("action"); // As action
        const ID = params.get("param"); // As id
        const ORDERTYPE = params.get("ordertype"); // As ordertype
        let HEADER_ACTION = '<%= Session("headerAction") %>';
        let CUSTOMERID = '<%= Session("CustomerId") %>';
        let USERNAME = '<%= Session("UserName") %>';
        let ROLENAME = '<%= Session("RoleName") %>';
        let LEVELNAME = '<%= Session("LevelName") %>';
        let LOGINID = '<%= Session("LoginId") %>';
        let URIMETHOD = '/Methods/Order/CreateOrderMethod.aspx';      
    </script>
    <script src="/Scripts/Order/CreateOrders.js?v=1.0.6"></script>

</asp:Content>

