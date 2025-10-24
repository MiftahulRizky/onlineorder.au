<%@ Page Title="Surcharge" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Surcharge_Default" MaintainScrollPositionOnPostback="true" Debug="true"%>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Surcharge</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="card col-12" id="card-table">
                    <div class="card-header row">
                        <div class="col-lg-6">
                            <h3 class="card-title">Data Surcharge</h3>
                        </div>
                        <div class="col-lg-2">
                            <select name="designid" id="designid" class="form-select"></select>
                        </div>
                        <div class="col-lg-2">
                            <select name="blindid" id="blindid" class="form-select"></select>
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-primary w-100" id="btn-add">
                                <i class="ti ti-plus opacity-50 fs-2 me-1"></i>
                                Create New
                            </button>
                        </div>
                    </div>
                    <div class="card-body" id="table-default">
                        <table class="table table-hover table-vcenter  card-table w-100" id="data-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>product</th>
                                    <th>blind no</th>
                                    <th>name</th>
                                    <th>formula</th>
                                    <th>charge</th>
                                    <th>action</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <!-- modalSubmit -->
    <div class="modal fade" id="modalSubmit" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalSubmitLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title " id="modalSubmitLabel">Modal title</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <form action="javascript:void(0)" method="post" id="form-submit">

                    <div class="modal-body">
                        <div class="row">
                            <div class="col-lg-5 mb-3">
                                <label for="designtype" class="form-label text-uppercase">design type</label>
                                <select name="designtype" id="designtype" class="form-select"></select>
                                <input type="text" class="form-control" name="id" id="id" readonly hidden>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="blindtype" class="form-label text-uppercase">blind type</label>
                                <select name="blindtype" id="blindtype" class="form-select"></select>
                            </div>
                            <div class="col-lg-3 mb-3">
                                <label for="blindno" class="form-label text-uppercase">blind no</label>
                                <select name="blindno" id="blindno" class="form-select">
                                    <option value=""></option>
                                    <option value="Blind 1">BLIND 1</option>
                                    <option value="Blind 2">BLIND 2</option>
                                    <option value="Blind 3">BLIND 3</option>
                                    <option value="Blind 4">BLIND 4</option>
                                </select>
                            </div>
                            <div class="col-lg-12 mb-3">
                                <label for="name" class="form-label text-uppercase">surcharge name</label>
                                <input type="text" class="form-control" name="name" id="name" placeholder="Surcharge Name ...">
                            </div>
                            <div class="col-lg-6 mb-1">
                                <label for="fieldname" class="form-label text-uppercase">formula</label>
                                <select name="fieldname" id="fieldname" class="form-select"></select>
                            </div>
                            <div class="col-lg-6 mb-1">
                                <label for="charge" class="form-label text-uppercase">charge</label>
                                <input type="text" class="form-control" name="charge" id="charge" placeholder="Charge ...">
                            </div>
                            <div class="col-lg-12 mb-3">
                                <textarea name="formula" id="formula" class="form-control " rows="6" cols="6" placeholder="Formula ..."></textarea>
                            </div>
                            <div class="col-lg-10 mb-3">
                                <label for="des" class="form-label text-uppercase">description</label>
                                <input type="text" class="form-control" name="des" id="des" placeholder="Description ...">
                            </div>
                            <div class="col-lg-2 mb-3">
                                <label for="active" class="form-label text-uppercase">active</label>
                                <select name="active" id="active" class="form-select">
                                    <option value="1">YES</option>
                                    <option value="0">NO</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" id="btn-submit">Submit</button>
                    </div>
                </form>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        let userId = '<%= Session("userId") %>';
        let userName = '<%= Session("UserName") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';  
        let storeId = '<%= Session("StoreId") %>';
        let storeCompany = '<%= Session("StoreCompany") %>';
        let levelName = '<%= Session("LevelName") %>';
        let uriMethod = '/Methods/Setting/Surcharge/SurchargeMethod.aspx'; 

    </script>
    <script src="/Scripts/Setting/Surcharge/Default.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

