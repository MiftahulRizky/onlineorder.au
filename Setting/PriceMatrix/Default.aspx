<%@ Page Title="Price Matrix" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_PriceMatrix_Default" MaintainScrollPositionOnPostback="true" Debug="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                    <div class="page-pretitle">Setting / Master Price</div>
                    <h2 class="page-title">Price Matrix</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">

            <div class="row">
                <div class="col-12">

                    <div class="card" id="cardTable">
                        <div class="card-header">
                            <h3 class="card-title col-lg-9">
                                <i class="fa-solid fa-table me-2"></i>
                                Data Price Matrix
                            </h3>
                            <div class="col-lg-3">
                                <%-- <div class="dropdown">
                                    <button class="btn btn-azure float-end dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                        <i class="fa-solid fa-plus me-2"></i>Add New
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li>
                                            <a class="dropdown-item" href="javascript:void(0)" id="btnInsert">
                                                <i class="bi bi-plus-lg opacity-50 me-2"></i>Insert
                                            </a>
                                        </li>
                                        <li>
                                            <a class="dropdown-item" href="javascript:void(0)" id="btnImport">
                                                <i class="bi bi-filetype-csv opacity-50 me-2"></i>
                                                Import CSV
                                            </a>
                                        </li>
                                    </ul>
                                </div> --%>
                                <button type="button" class="btn btn-azure float-end me-2" id="btnInsert">
                                    <i class="fa-solid fa-plus me-2"></i> Add New
                                </button>
                                <button type="button" class="btn btn-primary float-end me-2" id="btnFilter">
                                    <i class="fa-solid fa-filter me-2"></i>
                                    Filters
                                </button>
                            </div>
                        </div>
                        <div class="card-body">
                           <div id="table-default" class="table-responsive">
                                <table class="table table-vcenter table-hover card-table" id="tableAjax"  width="100%">
                                 <thead>
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3">GROUP NAME</th>
                                         <th class="h3">TYPE</th>
                                         <th class="h3">WIDTH</th>
                                         <th class="h3">DROP</th>
                                         <th class="h3">COST</th>
                                         <th class="text-center">ACTION</th>
                                     </tr>
                                 </thead>
                                 <tbody></tbody>
                                 <thead>
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3">GROUP NAME</th>
                                         <th class="h3">TYPE</th>
                                         <th class="h3">WIDTH</th>
                                         <th class="h3">DROP</th>
                                         <th class="h3">COST</th>
                                         <th class="text-center">ACTION</th>
                                     </tr>
                                 </thead>
                                </table>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>

     <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasFilter" aria-labelledby="canvasFilterLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasFilterLabel"> <i class="fa-solid fa-filter me-2"></i> Filter</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">

            <div class="row mb-3">
                <label for="designid" class="form-label">Design Type</label>
                <select class="form-select " id="designid" name="designid"></select>
            </div>

            <div class="row mb-3">
                <label for="pricegroupid" class="form-label">Group Name</label>
                <select class="form-select " id="pricegroupid" name="pricegroupid"></select>
            </div>

            <div class="row mb-3">
                <label for="type" class="form-label">Type</label>
                <select class="form-select " id="type" name="type">
                    <option value="Pick Up">PICK UP</option>
                    <option value="Delivery">DELIVERY</option>
                    <option value="INT-PU">INT-PU</option>
                    <option value="INT-FIS">INT-FIS</option>
                </select>
            </div>

            <div class="row mb-3">
                <label for="width" class="form-label">Width</label>
                <input type="number" min="0" class="form-control " id="width" name="width">
            </div>

            <div class="row mb-3">
                <label for="drop" class="form-label">Drop</label>
                <input type="number" min="0" class="form-control " id="drop" name="drop">
            </div>
            
            <div class="row mt-5 gap-1">
                <button class="btn mb-2 btn-primary" type="button" id="btnSearchFilter">
                    <i class="fa-solid fa-magnifying-glass me-2"></i> Search
                </button>
                <button class="btn mb-2" type="button" id="btnImport">
                     <i class="bi bi-filetype-csv me-2"></i> Import
                </button>
                <button class="btn mb-3 text-red" type="button" id="btnDelete">
                    <i class="fa-solid fa-trash me-2"></i> Delete
                </button>
            </div>
        </div>
    </div>





    <!-- Modal Import -->
    <div class="modal fade" id="modalImport" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalImportLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
            <div class="modal-header">
                <h1 class="modal-title" id="modalImportLabel">Modal title</h1>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body row">
                <div class="mb-3 col-lg-12">
                    <label for="fileupload" class="form-label">CSV File</label>
                    <input type="file" class="form-control " id="fileupload" name="fileupload" accept=".csv" />
                </div>
                <%-- <div class=" col-lg-6">
                    <label for="designid" class="form-label">Design Type</label>
                    <select class="form-select " id="designid" name="designid"></select>
                </div>

                <div class="mb-3 col-lg-12">
                    <label for="pricegroupid" class="form-label">Group Name</label>
                    <select class="form-select " id="pricegroupid" name="pricegroupid"></select>
                </div>

                <div class="mb-3 col-lg-12">
                    <label for="type" class="form-label">Type</label>
                    <select class="form-select " id="type" name="type">
                        <option value="Pick Up">PICK UP</option>
                        <option value="Delivery">DELIVERY</option>
                        <option value="INT-PU">INT-PU</option>
                        <option value="INT-FIS">INT-FIS</option>
                    </select>
                </div> --%>

            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                <i class="fa-solid fa-xmark me-2"></i>Close
                </button>
                <button type="button" class="btn btn-primary" id="submitImport">
                    <i class="fa-solid fa-cloud-arrow-up me-2"></i>Import Now
                </button>
            </div>
            </div>
        </div>
    </div>


    <!-- Modal modal -->
    <div class="modal fade" id="modalSaveData" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalSaveDataLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalSaveDataLabel">Modal title</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">
                    <div class="mb-3 col-md-12" id="divDesignType">
                        <label for="designid" class="form-label">Design Type</label>
                        <select class="form-select " id="designid" name="designid"></select>
                    </div>
                    
                    <div class="mb-3 col-md-12">
                        <label for="pricegroupid" class="form-label">Group Name</label>
                        <select class="form-select " id="pricegroupid" name="pricegroupid"></select>
                        <input type="text" class="form-control " id="id" name="id" placeholder="id for edit" readonly hidden>
                    </div>
                    
                    <div class="mb-3 col-lg-4">
                        <label for="type" class="form-label">Type</label>
                        <select class="form-select " id="type" name="type">
                            <option value="Pick Up">PICK UP</option>
                            <option value="Delivery">DELIVERY</option>
                            <option value="INT-PU">INT-PU</option>
                            <option value="INT-FIS">INT-FIS</option>
                        </select>
                    </div>

                    <div class="mb-3 col-md-4">
                        <label for="width" class="form-label">Width</label>
                        <input type="number" min="0" class="form-control " id="width" name="width">
                    </div>

                    <div class="mb-3 col-md-4">
                        <label for="drop" class="form-label">Drop</label>
                        <input type="number" min="0" class="form-control " id="drop" name="drop">
                    </div>

                    <div class="mb-3 col-md-12">
                        <label for="cost" class="form-label">Cost</label>
                        <input type="text" min="0" class="form-control " id="cost" name="cost">
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i> Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitSave">
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit 
                    </button>
                </div>
            </div>
        </div>
    </div>



    <script type="text/javascript">
        let userId = '<%= Session("userId") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';  
        let uriMethod = '/Methods/Setting/PriceMatrix';      
    </script>
    <script src="/Scripts/Setting/PriceMatrix/Default.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

