<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Order_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="List Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
                                <table class="table table-vcenter table-hover w-100" id="tableAjax">
                                 <thead>
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


    <!-- modalDateInfo -->
    <div class="modal fade" id="modalDateInfo" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalDateInfoLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalDateInfoLabel">Date Information</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">

                    <div class="mb-3 col-12">
                        <label for="createddate" class="form-label">1. Created Date</label>
                        <div class="input-icon">
                            <span class="input-icon-addon">
                                <i class="bi bi-calendar4"></i>
                            </span>
                            <input type="text" name="createddate" id="createddate" class="form-control" placeholder="Created Date ...." readonly>
                        </div>
                    </div>

                    <div class="mb-3 col-12">
                        <label for="submitteddate" class="form-label">2. Submitted Date</label>
                        <div class="input-icon">
                            <span class="input-icon-addon">
                                <i class="bi bi-calendar4"></i>
                            </span>
                            <input type="text" name="submitteddate" id="submitteddate" class="form-control" placeholder="Submitted Date ...." readonly>
                        </div>
                    </div>

                    <div class="mb-3 col-12">
                        <label for="completeddate" class="form-label">3. Completed Date</label>
                        <div class="input-icon">
                            <span class="input-icon-addon">
                                <i class="bi bi-calendar4"></i>
                            </span>
                            <input type="text" name="completeddate" id="completeddate" class="form-control" placeholder="Completed Date ...." readonly>
                        </div>
                    </div>

                    <div class=" col-12">
                        <label for="canceleddate" class="form-label">4. Canceled Date</label>
                        <div class="input-icon">
                            <span class="input-icon-addon">
                                <i class="bi bi-calendar4"></i>
                            </span>
                            <input type="text" name="canceleddate" id="canceleddate" class="form-control" placeholder="Canceled Date ...." readonly>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- modalChangeStatus -->
    <div class="modal fade" id="modalChangeStatus" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalChangeStatusLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalChangeStatusLabel">Update Status Order</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">

                    <div class="col-12 mb-3">
                        <label for="status" class="form-label text-uppercase">Status</label>
                        <select name="status" id="status" class="form-select"></select>

                        <input type="text" class="form-control " id="statusOld" name="statusOld"readonly hidden>

                        <input type="text" class="form-control " id="id" name="id" placeholder="id for edit" readonly hidden>
                    </div>

                    <div class="col-12 mb-3" id="divSubmittedDate">
                        <label for="submitteddate" class="form-label text-uppercase">Submitted Date</label>
                        <input type="date" class="form-control" id="submitteddate" name="submitteddate">
                    </div>

                    <div class="col-12 mb-3" id="divCompletedDate">
                        <label for="completeddate" class="form-label text-uppercase">Completed Date</label>
                        <input type="date" class="form-control " id="completeddate" name="completeddate">
                    </div>

                    <div class="col-12 mb-3" id="divCanceledDate">
                        <label for="canceleddate" class="form-label text-uppercase">Canceled Date</label>
                        <input type="date" class="form-control " id="canceleddate" name="canceleddate">
                    </div>

                    <div class="col-12 mb-3" id="divDescription">
                        <label for="description" class="form-label text-uppercase d-flex justify-content-between">
                            Description
                            <i class="bi bi-question-circle" style="cursor: pointer;" id="tooltipDescription"></i>
                        </label>
                        <textarea name="description" class="form-control" id="description" rows="3" ></textarea>
                    </div>


                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitChangeStatus">
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i>Submit
                    </button>
                </div>
            </div>
        </div>
    </div>



    <!-- logs -->
    <div class="modal fade" id="modalLogs" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalLogsLabel" aria-hidden="true">
        <div class="modal-dialog  modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalLogsLabel">Changelog</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <table class="table" width="100%" id="table-logs">
                        <tbody></tbody>
                        <!-- <tr>
                            <td>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</td>
                        </tr>
                        <tr>
                            <td>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</td>
                        </tr> -->
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
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
        let URIMETHOD = '/Methods/Order/DefaultMethod.aspx';      
        let src="../Scripts/OrderHeaderPage/OrderHeaderPage.js";
    </script>
    <script src="/Scripts/Order/Default.js?<%= DateTime.Now.Ticks %>"></script>


</asp:Content>