using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    /**
     * Id Payload used to send dataless payloads.
     */
    class Id : IPayload
    {
        public string id;

        public Id(string id)
        {
            this.id = id;
        }
    }
}
