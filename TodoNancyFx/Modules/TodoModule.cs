using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.Data;
using System.Dynamic;

namespace TodoNancyFx.Modules
{
    public class TodoModule : NancyModule
    {
        private readonly dynamic _db = Database.OpenNamedConnection("TodoNancyFx");

        public TodoModule()
        {
            Get["/"] = parameters =>
            {
                var tasks = new {
                    IsDone = _db.Tasks.FindAllBy(IsDone : true),
                    NotDone = _db.Tasks.FindAllBy(IsDone: false),
                };

                return View["Todo", tasks];
            };

            Get["/done/{TaskId}/{IsDone}"] = parameters =>
            {
                var taskId = parameters.TaskId;
                var isDone = parameters.IsDone;                

                var task = _db.Tasks.Get(taskId);
                task.IsDone = isDone;

                _db.Tasks.Update(task);

                return Response.AsRedirect("/");
            };

            Get["/delete/{TaskId}"] = parameters =>
            {
                _db.Tasks.Delete(TaskId : parameters.TaskId);
                return Response.AsRedirect("/");
            };

            Post["/"] = parameters =>
            {
                if (Request.Form.Task == string.Empty)
                {
                    return Response.AsRedirect("/"); ;
                }

                dynamic task = new ExpandoObject();
                task.TaskId = Guid.NewGuid();
                task.Task = Request.Form.Task;
                task.IsDone = false;

                _db.Tasks.Insert(task);

                return Response.AsRedirect("/");
            };
        }

    }
}