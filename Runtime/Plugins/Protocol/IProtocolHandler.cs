using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {
    public enum ProtocolID {

        PostTest

    }

    public interface IProtocolHandler {

        // 요청에 대한 response(결과) 를 처리하는 메서드
        void Process(ProtocolRes response);

    }
}
