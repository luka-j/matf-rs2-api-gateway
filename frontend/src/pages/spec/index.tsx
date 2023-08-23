import { useState } from "react";
import { Highlight, themes } from "prism-react-renderer";
import { useParams } from "react-router-dom";
import Editor from "react-simple-code-editor";
import SwaggerUI from "swagger-ui-react";

import "swagger-ui-react/swagger-ui.css";

import { cn } from "@/utils/style-utils";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";

const Spec = () => {
  const [code, setCode] = useState("");
  const params = useParams()["*"]?.split("/");
  console.log(params);

  const highlight = (code: string) => (
    <Highlight code={code} language="yaml" theme={themes.vsDark}>
      {({ className, style, tokens, getLineProps, getTokenProps }) => (
        <div style={style} className={cn(className, "")}>
          {tokens.map((line, i) => (
            <div key={i} {...getLineProps({ line, key: i })}>
              {line.map((token, key) => (
                <span key={key} {...getTokenProps({ token, key })} />
              ))}
            </div>
          ))}
        </div>
      )}
    </Highlight>
  );

  return (
    <div className="flex w-full flex-col">
      <div className="flex h-[80vh] w-full">
        <ScrollArea className=" w-1/2 rounded-md border">
          <Editor
            value={code}
            onValueChange={setCode}
            highlight={highlight}
            padding={10}
            style={themes.vsDark.plain}
          />
        </ScrollArea>

        <ScrollArea className=" w-1/2 rounded-md border">
          <SwaggerUI spec={code} />
        </ScrollArea>
      </div>
      <div className="mt-4 flex justify-center">
        <Button size="lg">Save & Publish</Button>
      </div>
    </div>
  );
};

export default Spec;
