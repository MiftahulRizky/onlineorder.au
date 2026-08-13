<%@ Page Title="Orders" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="OrderHeaders.aspx.vb" Inherits="Order_OrderHeaders" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">

                <div class="col-5">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">List Order</h2>
                </div>

                <div class="col-7 text-end">
                    <a href="javascript:void(0);" id="aDailyMail" class="btn btn-secondary">Daily Mail</a>
                    <a id="aOtorisasi" class="btn btn-info position-relative" href="javascript:void(0);">
                        Waiting for Authorization List
                        <span id="spanOtorisasi" class="badge bg-red text-blue-fg badge-notification badge-pill">10</span>
                    </a>
                </div>

            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-12">
                    <div class="card" id="cardOrder">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="ti ti-shopping-cart me-1 opacity-50 fs-2"></i>
                                Data Order
                            </h3>
                            <div class="card-actions d-flex">
                                <select name="status" id="status" class="form-select " style="width: 180px;"></select>
                                <select name="ordertype" id="ordertype" class="form-select ms-3" style="width: 180px;"></select>
                                <button type="button" class="btn btn-primary float-end ms-3" id="btnCreateNewOrder">
                                    <i class="fa-solid fa-plus me-2"></i>
                                    Create Order
                                </button>
                            </div>
                        </div>
                        <div class="card-header" id="divInfo">
                            <div class="mx-auto alert alert-warning mb-0" role="alert">
                                <div class="d-flex align-items-center">
                                    <div>
                                       <i class="ti ti-alert-square-rounded fs-2 me-2"></i>
                                    </div>
                                    <div>
                                        Please note that all draft orders will be removed from the system if there are no activities after 10 days. <b>Your order will go into order cancellation. </b>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <div id="table-default" class="">
                                <table class="table table-bordered table-vcenter table-hover w-100" id="tableOrders">
                                 <thead class="h1">
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3 column-id">ID</th>
                                         <th class="h3">ORDER ID</th>
                                         <th class="h3 column-retailer">RETAILER NAME</th>
                                         <th class="h3">ORDER NUMBER</th>
                                         <th class="h3">ORDER NAME</th>
                                         <th class="h3 column-type">ORDER TYPE</th>
                                         <th class="h3">DELIVERY</th>
                                         <th class="h3">STATUS</th>
                                         <th class="h3 text-center">CREATED</th>
                                         <th class="h3 text-center">SUBMITTED</th>
                                         <th class="text-center">ACTIONS</th>
                                     </tr>
                                 </thead>
                                 <tbody></tbody>
                                 <thead>
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3 column-id">ID</th>
                                         <th class="h3">ORDER ID</th>
                                         <th class="h3 column-retailer">RETAILER NAME</th>
                                         <th class="h3">ORDER NUMBER</th>
                                         <th class="h3">ORDER NAME</th>
                                         <th class="h3 column-type">ORDER TYPE</th>
                                         <th class="h3 ">DELIVERY</th>
                                         <th class="h3 ">STATUS</th>
                                         <th class="h3 text-center">CREATED</th>
                                         <th class="h3 text-center">SUBMITTED</th>
                                         <th class="text-center">ACTIONS</th>
                                     </tr>
                                 </thead>
                                </table>
                            </div>
                        </div>
                        <div class="card-footer">
                            <div class="row">
                                <div class="col-8"></div>
                                <div class="col-2">
                                    <select name="active" id="active" class="form-select">
                                        <option value="1">ACTIVE DATA</option>
                                        <option value="0">NON ACTIVE</option>
                                    </select>
                                    <small class="form-hint">* Sort Data</small>
                                </div>
                                <div class="col-2">
                                    <select name="storetype" id="storetype" class="form-select">
                                        <option value="ALL">ALL</option>
                                        <option value="REGULAR">REGULAR</option>
                                        <option value="PRO FORMA">PRO FORMA</option>
                                    </select>
                                    <small class="form-hint">* Store Type</small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let CUSTOMERACCOUNT = '<%= Session("CustomerAccount") %>';
        let CUSTOMERID = '<%= Session("CustomerId") %>';
        let SESSION_SP = '<%= Session("Sunlight Product") %>';
        let ONSTOP = '<%= Session("OnStop") %>';
        let USERNAME = '<%= Session("UserName") %>';
        let LOGINID = '<%= Session("LoginId") %>';
        let ROLENAME = '<%= Session("RoleName") %>';  
        let FULLNAME = '<%= Session("FullName") %>';
        let CUSTOMERCOMPANY = '<%= Session("CustomerCompany") %>';
        let LEVELNAME = '<%= Session("LevelName") %>';
        let URIMETHOD = '/Methods/Order/OrderHeaderMethod.aspx';      
    </script>
    <script src="/Scripts/Order/OrderHeaders.js?v=1.0.10"></script>
</asp:Content>

